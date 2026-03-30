import { useState, useRef } from "react";

export default function ApplicationModal(props) {
    const initialFormData = {
        bedrijf: "",
        functie: "",
        jobUrl: "",
        status: "",
        datum: "",
        locatie: "",
        salaris: "",
        contactpersoon: "",
        contactEmail: "",
        volgendeStap: "",
        beschrijving: "",
    };
    const formTopRef = useRef(null);

    const [formData, setFormData] = useState(initialFormData);
    const [errors, setErrors] = useState({});

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

    function handleSubmit(event) {
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

        setErrors({});
        console.log(formData);

        setFormData(initialFormData);
        props.onClose();
    }

    function handleClose() {
        setFormData(initialFormData);
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

    return (
        <section className="application-modal-overlay" onClick={handleClose}>
            <div className="application-modal">
                <div ref={formTopRef} className="application-modal-header">
                    <h2 className="application-modal-title">Nieuwe sollicitatie toevoegen</h2>
                    <button
                        type="button"
                        className="application-modal-close"
                        onClick={handleClose}
                        aria-label="Sluit modal"
                    >
                        x
                    </button>
                </div>

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
                            <input
                                type="text"
                                name="status"
                                value={formData.status}
                                onChange={handleChange}
                                className={errors.status ? "input-error" : ""}
                            />
                            {errors.status && <p className="field-error">{errors.status}</p>}
                        </div>

                        <div className="application-modal-field">
                            <label>Datum *</label>
                            <input
                                type="text"
                                name="datum"
                                value={formData.datum}
                                onChange={handleChange}
                                placeholder="bijv. 15 mrt 2026"
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
                            <label>Salaris</label>
                            <input
                                type="text"
                                name="salaris"
                                value={formData.salaris}
                                onChange={handleChange}
                                placeholder="bijv. EUR 40.000 - EUR 50.000"
                            />
                        </div>

                        <div className="application-modal-field">
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
                        </div>
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
                        >
                            Toevoegen
                        </button>
                    </div>
                </form>
            </div>
        </section>
    );
}
