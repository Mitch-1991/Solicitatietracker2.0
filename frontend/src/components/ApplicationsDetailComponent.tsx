import { useEffect, useState } from "react";
import { mapApplicationToFormData } from "../mappers/applicationMappers.ts";
import type {ApplicationFormData, ApplicationDetailResponse} from "../types/application.ts";

type ApplicationDetailProps = {
    initialApplication: ApplicationDetailResponse | null;
    onClose: () => void;
};
export default function ApplicationDetail(props: ApplicationDetailProps) {

    const [formData, setFormData] = useState<ApplicationFormData>(() => mapApplicationToFormData(props.initialApplication));

    useEffect(() => {
        setFormData(mapApplicationToFormData(props.initialApplication));
    }, [props.initialApplication]);

    function handleClose(): void {
        props.onClose();
    }

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
                        <p><strong>Bedrijf:</strong> {formData.companyName}</p>
                        <p><strong>Jobtitel:</strong> {formData.jobTitle}</p>
                        <p><strong>Status:</strong> {formData.status}</p>
                        <p><strong>Applied date:</strong> {formData.date || "-"}</p>
                        <p><strong>Volgende stap:</strong> {formData.nextStep || "-"}</p>

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

    )
}