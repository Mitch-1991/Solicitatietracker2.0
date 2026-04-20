import { useState } from "react";
import { mapApplicationToFormData } from "../mappers/SollicitatieMappers";
export default function ApplicationDetail(props) {

    const [formData, setFormData] = useState(() => mapApplicationToFormData(props.initialApplication));

    function handleClose() {
        props.onClose();
    }
    console.log(props.initialApplication)
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
                        <p><strong>Bedrijf:</strong> {formData.bedrijf}</p>
                        <p><strong>Jobtitel:</strong> {formData.functie}</p>
                        <p><strong>Status:</strong> {formData.status}</p>
                        <p><strong>Applied date:</strong> {formData.datum || "-"}</p>
                        <p><strong>Volgende stap:</strong> {formData.volgendeStap || "-"}</p>

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