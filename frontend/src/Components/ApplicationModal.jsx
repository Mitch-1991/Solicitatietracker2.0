import { useEffect, useRef, useState } from "react";
import { createApplication, updateApplication } from "../Services/SollicitatieService";
import {
    mapCreatedApplicationToOverviewItem,
    mapFormDataToCreateDto,
    mapFormDataToUpdateDto
} from "../mappers/SollicitatieMappers";

const emptyFormData = {
    bedrijf: "",
    functie: "",
    jobUrl: "",
    status: "",
    datum: "",
    priority: "",
    locatie: "",
    salarisMin: "",
    salarisMax: "",
    contactpersoon: "",
    contactEmail: "",
    volgendeStap: "",
    beschrijving: "",
};

function mapApplicationToFormData(application) {
    if (!application) {
        return emptyFormData;
    }

    return {
        bedrijf: application.bedrijf ?? "",
        functie: application.functie ?? application.jobTitle ?? "",
        jobUrl: application.jobUrl ?? "",
        status: application.status ?? "",
        datum: application.datum ?? application.appliedDate ?? "",
        priority: application.priority ?? "",
        locatie: application.locatie ?? application.location ?? "",
        salarisMin: application.salarisMin?.toString() ?? application.salaryMin?.toString() ?? "",
        salarisMax: application.salarisMax?.toString() ?? application.salaryMax?.toString() ?? "",
        contactpersoon: application.contactpersoon ?? application.contactPerson ?? "",
        contactEmail: application.contactEmail ?? "",
        volgendeStap: application.volgendeStap ?? application.nextStep ?? "",
        beschrijving: application.beschrijving ?? application.omschrijving ?? "",
    };
}

export default function ApplicationModal(props) {
    const formTopRef = useRef(null);
    const isEditMode = props.mode === "edit";
    const modalTitle = isEditMode ? "Sollicitatie bewerken" : "Nieuwe sollicitatie toevoegen";
    const submitLabel = isEditMode ? "Wijzigingen opslaan" : "Opslaan";
    const submittingLabel = isEditMode ? "Opslaan..." : "Verwerken...";

    const [formData, setFormData] = useState(() => mapApplicationToFormData(props.initialApplication));
    const [errors, setErrors] = useState({});
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [serverError, setServerError] = useState("");
    const [createdApplication, setCreatedApplication] = useState(null);

    useEffect(() => {
        setFormData(mapApplicationToFormData(props.initialApplication));
        setErrors({});
        setServerError("");
        setCreatedApplication(null);
        setIsSubmitting(false);
    }, [props.initialApplication, props.mode]);

    function handleChange(event) {
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
    function validateForm() {
        const newErrors = {};

        if (!formData.bedrijf.trim()) {
            newErrors.bedrijf = "Bedrijf is verplicht.";
        }

        if (!formData.functie.trim()) {
            newErrors.functie = "Functie is verplicht.";
        }

        if (!formData.status.trim()) {
            newErrors.status = "Status is verplicht.";
        }

        if (!formData.datum.trim()) {
            newErrors.datum = "Datum is verplicht.";
        }
        if (formData.jobUrl.trim() && !isValidUrl(formData.jobUrl.trim())) {
            newErrors.jobUrl = "Voer een geldige URL in.";
        }
        if (formData.contactEmail.trim() && !isValidEmail(formData.contactEmail.trim())) {
            newErrors.contactEmail = "Voer een geldig e-mailadres in.";
        }

        return newErrors;
    }

    async function handleSubmit(event) {
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
                const applicationId = props.initialApplication.id

                if(!applicationId){
                    throw new Error("Ongeldige sollicitatie. ID ontbreekt.")
                }
                const dto = mapFormDataToUpdateDto(formData)
                const updatedResult = await updateApplication(applicationId, dto)

                const updatedApplicationForOverview = mapCreatedApplicationToOverviewItem({
                    ...updatedResult,
                    appliedDate: updatedResult.appliedDate ?? dto.appliedDate,
                    nextStep: updatedResult.nextStep ?? dto.nextStep,
                },
                updatedResult.bedrijf ?? dto.bedrijf
                )

                props.onUpdated?.(updatedApplicationForOverview)
                handleClose()
                return;
            }

            const dto = mapFormDataToCreateDto(formData)
            const createdResult = await createApplication(dto)
            const createdApplicationWithCompany = {
                ...createdResult,
                bedrijf: dto.bedrijf,
                appliedDate: createdResult.appliedDate ?? dto.appliedDate,
                nextStep: createdResult.nextStep ?? dto.nextStep,
            };

            setCreatedApplication(createdApplicationWithCompany)

            if (props.onCreated) {
                props.onCreated(
                    mapCreatedApplicationToOverviewItem(createdApplicationWithCompany, dto.bedrijf)
                )
            }
        } catch (error) {
            setServerError(error.message || "Er is een fout opgetreden bij het aanmaken van de sollicitatie.")

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

    function isValidUrl(value) {
        try {
            new URL(value);
            return true;
        } catch {
            return false;
        }
    }
    function isValidEmail(value) {
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
                            <p><strong>Bedrijf:</strong> {createdApplication.bedrijf}</p>
                            <p><strong>Jobtitel:</strong> {createdApplication.jobTitle}</p>
                            <p><strong>Status:</strong> {createdApplication.status}</p>
                            <p><strong>Prioriteit:</strong> {createdApplication.priority || "-"}</p>
                            <p><strong>Applied date:</strong> {createdApplication.appliedDate || "-"}</p>
                            <p><strong>Volgende stap:</strong> {createdApplication.nextStep || "-"}</p>
                            <p><strong>Aangemaakt op:</strong> {createdApplication.createdAt}</p>
                            <p><strong>Updated op:</strong> {createdApplication.updatedAt}</p>
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
                                name="bedrijf"
                                value={formData.bedrijf}
                                onChange={handleChange}
                                className={errors.bedrijf ? "input-error" : ""}
                            />
                            {errors.bedrijf && <p className="field-error">{errors.bedrijf}</p>}
                        </div>
                        <div className="application-modal-field">
                            <label>Functie *</label>
                            <input
                                type="text"
                                name="functie"
                                value={formData.functie}
                                onChange={handleChange}
                                className={errors.functie ? "input-error" : ""}
                            />
                            {errors.functie && <p className="field-error">{errors.functie}</p>}
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
                                name="datum"
                                defaultValue={Date.now}
                                value={formData.datum}
                                onChange={handleChange}
                                className={errors.datum ? "input-error" : ""}
                            />
                            {errors.datum && <p className="field-error">{errors.datum}</p>}
                        </div>

                        <div className="application-modal-field">
                            <label>Job URL</label>
                            <input
                                type="text"
                                name="jobUrl"
                                value={formData.jobUrl}
                                onChange={handleChange}

                            />
                            {errors.jobUrl && <p className="field-error">{errors.jobUrl}</p>}
                        </div>

                        <div className="application-modal-field">
                            <label>Locatie</label>
                            <input
                                type="text"
                                name="locatie"
                                value={formData.locatie}
                                onChange={handleChange}
                            />
                        </div>

                        <div className="application-modal-field">
                            <label>Salaris min</label>
                            <input
                                type="text"
                                name="salarisMin"
                                value={formData.salarisMin}
                                onChange={handleChange}
                            />
                        </div>
                        <div className="application-modal-field">
                            <label>Salaris max</label>
                            <input
                                type="text"
                                name="salarisMax"
                                value={formData.salarisMax}
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

                        {/* <div className="application-modal-field">
                            <label>Contactpersoon</label>
                            <input
                                type="text"
                                name="contactpersoon"
                                value={formData.contactpersoon}
                                onChange={handleChange}
                            />
                        </div>

                        <div className="application-modal-field">
                            <label>Contact e-mail</label>
                            <input
                                type="email"
                                name="contactEmail"
                                value={formData.contactEmail}
                                onChange={handleChange}
                            />
                            {errors.contactEmail && <p className="field-error">{errors.contactEmail}</p>}
                        </div> */}
                    </div>

                    <div className="application-modal-field application-modal-field-full">
                        <label>Volgende stap</label>
                        <input
                            type="text"
                            name="volgendeStap"
                            value={formData.volgendeStap}
                            onChange={handleChange}
                            placeholder="bijv. Wachten op reactie"
                        />
                    </div>

                    <div className="application-modal-field application-modal-field-full">
                        <label>Beschrijving</label>
                        <textarea
                            name="beschrijving"
                            value={formData.beschrijving}
                            onChange={handleChange}
                            rows="4"
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
