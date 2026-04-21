import React, { useEffect, useRef, useState } from "react";
import { createApplication, updateApplication } from "../services/applicationService.ts";
import {
    emptyFormData,
    mapCreatedApplicationToOverviewItem,
    mapFormDataToCreateDto,
    mapFormDataToUpdateDto,
    mapApplicationToFormData,
} from "../mappers/applicationMappers";
import type { 
    ApplicationDetailResponse, 
    ApplicationFormData, 
    createdApplicationResponse, 
    updateApplicationDto ,
    createApplicationDto
} from "../types/application.ts";
import type { DashboardOverviewItem } from "../types/dashboard.ts";


type ApplicationModalProps = {
    mode: "create" | "edit";
    initialApplication: ApplicationDetailResponse | null;
    onClose: () => void;
    onCreated?: (createdOverviewItem: DashboardOverviewItem) => void;
    onUpdated?: (updatedOverviewItem: DashboardOverviewItem) => void;
};
type ApplicationFormErrors = Partial<Record<keyof ApplicationFormData, string>>;

export default function ApplicationModal(props: ApplicationModalProps) {
    const formTopRef = useRef<HTMLDivElement | null>(null);
    const isEditMode: boolean = props.mode === "edit";
    const modalTitle: string = isEditMode ? "Sollicitatie bewerken" : "Nieuwe sollicitatie toevoegen";
    const submitLabel: string = isEditMode ? "Wijzigingen opslaan" : "Opslaan";
    const submittingLabel: string = isEditMode ? "Opslaan..." : "Verwerken...";

    const [formData, setFormData] = useState<ApplicationFormData>(() => mapApplicationToFormData(props.initialApplication));
    const [errors, setErrors] = useState<ApplicationFormErrors>({});
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [serverError, setServerError] = useState("");
    const [createdApplication, setCreatedApplication] = useState<ApplicationFormData | null>(null);

    useEffect(() => {
        setFormData(mapApplicationToFormData(props.initialApplication));
        setErrors({});
        setServerError("");
        setCreatedApplication(null);
        setIsSubmitting(false);
    }, [props.initialApplication, props.mode]);

    function handleChange(event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) {
        const { name, value } = event.target;

        setFormData((prevData) => ({
            ...prevData,
            [name]: value,
        }));

        setErrors((prevErrors) => ({
            ...prevErrors,
            [name]: "",
        }));
    }
    function validateForm(): ApplicationFormErrors {
        const newErrors: ApplicationFormErrors = {};

        if (!formData.companyName.trim()) {
            newErrors.companyName = "Bedrijf is verplicht.";
        }

        if (!formData.jobTitle.trim()) {
            newErrors.jobTitle = "Functie is verplicht.";
        }

        if (!formData.status.trim()) {
            newErrors.status = "Status is verplicht.";
        }

        if (!formData.date?.trim()) {
            newErrors.date = "Datum is verplicht.";
        }
        if (formData.jobUrl?.trim() && !isValidUrl(formData.jobUrl.trim())) {
            newErrors.jobUrl = "Voer een geldige URL in.";
        }
        if (formData.contactEmail.trim() && !isValidEmail(formData.contactEmail.trim())) {
            newErrors.contactEmail = "Voer een geldig e-mailadres in.";
        }

        return newErrors;
    }

    async function handleSubmit(event: React.SubmitEvent<HTMLFormElement>): Promise<void> {
        event.preventDefault();

        const validationErrors = validateForm();
        if (Object.keys(validationErrors).length > 0) {
            setErrors(validationErrors);

            formTopRef.current?.scrollIntoView({
                behavior: "smooth",
                block: "start",
            })
            return;
        }

        try {
            setIsSubmitting(true)
            setServerError("")
            setErrors({});

            if (isEditMode) {
                if (!props.initialApplication) {
                    throw new Error("Ongeldige sollicitatie. Geen gegevens om te bewerken.")
                }
                const applicationId: number = props.initialApplication.id

                if(!applicationId){
                    throw new Error("Ongeldige sollicitatie. ID ontbreekt.")
                }
                const dto: updateApplicationDto = mapFormDataToUpdateDto(formData)
                const updatedResult: createdApplicationResponse = await updateApplication(applicationId, dto)

                const updatedApplicationForOverview: DashboardOverviewItem = mapCreatedApplicationToOverviewItem({
                    ...updatedResult,
                    appliedDate: updatedResult.appliedDate ?? dto.appliedDate,
                    nextStep: updatedResult.nextStep ?? dto.nextStep,
                },
                updatedResult.companyName ?? dto.companyName
                )

                props.onUpdated?.(updatedApplicationForOverview)
                handleClose()
                return;
            }

            const dto: createApplicationDto = mapFormDataToCreateDto(formData)
            const createdResult: createdApplicationResponse = await createApplication(dto)
            const createdApplicationWithCompany: ApplicationFormData = mapApplicationToFormData({
                ...createdResult,
                companyName: createdResult.companyName ?? dto.companyName,
                appliedDate: createdResult.appliedDate ?? dto.appliedDate,
                nextStep: createdResult.nextStep ?? dto.nextStep,
            }  
        );

            setCreatedApplication(createdApplicationWithCompany)

            if (props.onCreated) {
                props.onCreated(mapCreatedApplicationToOverviewItem(createdResult, createdResult.companyName ?? dto.companyName));
            }
        } catch (error: unknown) {
            setServerError(error instanceof Error ? error.message : "Er is een fout opgetreden bij het aanmaken van de sollicitatie.")

            formTopRef.current?.scrollIntoView({
                behavior: "smooth",
                block: "start",
            })
        } finally {
            setIsSubmitting(false)
        }

    }

    function handleClose() {
        setFormData(emptyFormData);
        setServerError("")
        setErrors({});
        setCreatedApplication(null)
        props.onClose();
    }

    function isValidUrl(value: string): boolean {
        try {
            new URL(value);
            return true;
        } catch {
            return false;
        }
    }
    function isValidEmail(value: string): boolean {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
    }

    if (createdApplication && !isEditMode) {
        return (
            <section
                className="application-modal-overlay"
                onClick={(e) => e.target === e.currentTarget && handleClose()}
            >
                <div className="application-modal">
                    <div className="application-modal-header">
                        <h2 className="application-modal-title">Sollicitatie succesvol aangemaakt</h2>
                        <button
                            type="button"
                            className="application-modal-close"
                            onClick={handleClose}
                            aria-label="Sluit modal"
                        >
                            x
                        </button>
                    </div>

                    <div className="application-modal-form">
                        <div className="application-modal-field application-modal-field-full">
                            <p><strong>Bedrijf:</strong> {createdApplication.companyName}</p>
                            <p><strong>Jobtitel:</strong> {createdApplication.jobTitle}</p>
                            <p><strong>Status:</strong> {createdApplication.status}</p>
                            <p><strong>Prioriteit:</strong> {createdApplication.priority || "-"}</p>
                            <p><strong>Applied date:</strong> {createdApplication.date || "-"}</p>
                            <p><strong>Volgende stap:</strong> {createdApplication.nextStep || "-"}</p>

                        </div>

                        <div className="application-modal-footer">
                            <button
                                type="button"
                                className="application-modal-button application-modal-button-primary"
                                onClick={handleClose}
                            >
                                Sluiten
                            </button>
                        </div>
                    </div>
                </div>
            </section>
        );
    }

    return (
        <section className="application-modal-overlay" onClick={(e) => e.target === e.currentTarget && handleClose()}>
            <div className="application-modal">
                <div ref={formTopRef} className="application-modal-header">
                    <h2 className="application-modal-title">{modalTitle}</h2>
                    <button
                        type="button"
                        className="application-modal-close"
                        onClick={handleClose}
                        aria-label="Sluit modal"
                    >
                        x
                    </button>
                </div>
                {serverError && (
                    <div className="field-error" style={{ marginBottom: "1rem" }}>
                        {serverError}
                    </div>
                )}

                <form className="application-modal-form" onSubmit={handleSubmit}>
                    <div className="application-modal-grid">
                        <div className="application-modal-field">
                            <label>Bedrijf *</label>
                            <input
                                type="text"
                                name="companyName"
                                value={formData.companyName}
                                onChange={handleChange}
                                className={errors.companyName ? "input-error" : ""}
                            />
                            {errors.companyName && <p className="field-error">{errors.companyName}</p>}
                        </div>
                        <div className="application-modal-field">
                            <label>Functie *</label>
                            <input
                                type="text"
                                name="jobTitle"
                                value={formData.jobTitle}
                                onChange={handleChange}
                                className={errors.jobTitle ? "input-error" : ""}
                            />
                            {errors.jobTitle && <p className="field-error">{errors.jobTitle}</p>}
                        </div>

                        <div className="application-modal-field">
                            <label>Status *</label>
                            <select
                                name="status"
                                value={formData.status}
                                onChange={handleChange}
                                className={errors.status ? "input-error" : ""}
                            >
                                <option value="">Selecteer status</option>
                                <option value="Verzonden">Verzonden</option>
                                <option value="Gesprek">Gesprek</option>
                                <option value="Afgewezen">Afgewezen</option>
                                <option value="Aanbieding">Aanbieding</option>
                            </select>
                            {errors.status && <p className="field-error">{errors.status}</p>}
                        </div>



                        <div className="application-modal-field">
                            <label>Datum *</label>
                            <input
                                type="date"
                                name="date"
                                defaultValue={Date.now()}
                                value={formData.date || ""}
                                onChange={handleChange}
                                className={errors.date ? "input-error" : ""}
                            />
                            {errors.date && <p className="field-error">{errors.date}</p>}
                        </div>

                        <div className="application-modal-field">
                            <label>Job URL</label>
                            <input
                                type="text"
                                name="jobUrl"
                                value={formData.jobUrl || ""}
                                onChange={handleChange}

                            />
                            {errors.jobUrl && <p className="field-error">{errors.jobUrl}</p>}
                        </div>

                        <div className="application-modal-field">
                            <label>Locatie</label>
                            <input
                                type="text"
                                name="location"
                                value={formData.location || ""}
                                onChange={handleChange}
                            />
                        </div>

                        <div className="application-modal-field">
                            <label>Salaris min</label>
                            <input
                                type="text"
                                name="salaryMin"
                                value={formData.salaryMin}
                                onChange={handleChange}
                            />
                        </div>
                        <div className="application-modal-field">
                            <label>Salaris max</label>
                            <input
                                type="text"
                                name="salaryMax"
                                value={formData.salaryMax}
                                onChange={handleChange}
                            />
                        </div>
                        <div className="application-modal-field">
                            <label>Prioriteit</label>
                            <select
                                name="priority"
                                value={formData.priority}
                                onChange={handleChange}
                                className={formData.priority ? "" : "application-modal-select-placeholder"}
                            >
                                <option value="">Selecteer prioriteit</option>
                                <option value="hoog">Hoog</option>
                                <option value="gemiddeld">Gemiddeld</option>
                                <option value="laag">Laag</option>
                            </select>
                        </div>
                    </div>

                    <div className="application-modal-field application-modal-field-full">
                        <label>Volgende stap</label>
                        <input
                            type="text"
                            name="nextStep"
                            value={formData.nextStep}
                            onChange={handleChange}
                            placeholder="bijv. Wachten op reactie"
                        />
                    </div>

                    <div className="application-modal-field application-modal-field-full">
                        <label>Beschrijving</label>
                        <textarea
                            name="notes"
                            value={formData.notes}
                            onChange={handleChange}
                            rows={4}
                        />
                    </div>

                    <div className="application-modal-footer">
                        <button
                            type="button"
                            className="application-modal-button application-modal-button-secondary"
                            onClick={handleClose}
                        >
                            Annuleren
                        </button>
                        <button
                            type="submit"
                            className="application-modal-button application-modal-button-primary"
                            disabled={isSubmitting}
                        >
                            {isSubmitting ? submittingLabel : submitLabel}
                        </button>
                    </div>
                </form>
            </div>
        </section>
    );
}
