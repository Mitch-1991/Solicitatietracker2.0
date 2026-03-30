import { useState } from "react";

export default function ApplicationModal(props) {
    const initialFormData = {
        bedrijf: "",
        functie: "",
        status: "",
        datum: "",
        locatie: "",
        salaris: "",
        contactpersoon: "",
        contactEmail: "",
        volgendeStap: "",
        beschrijving: "",
    };

    const [formData, setFormData] = useState(initialFormData);

    function handleChange(event) {
        const { name, value } = event.target;

        setFormData((prevData) => ({
            ...prevData,
            [name]: value,
        }));
    }

    function handleSubmit(event) {
        event.preventDefault();

        if (!formData.bedrijf || !formData.functie || !formData.status || !formData.datum) {
            alert("Vul alle verplichte velden in.");
            return;
        }

        console.log(formData);

        setFormData(initialFormData);
        props.onClose();
    }

    function handleClose() {
        setFormData(initialFormData);
        props.onClose();
    }

    return (
        <section className="application-modal-overlay">
            <div className="application-modal">
                <div className="application-modal-header">
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
                            />
                        </div>

                        <div className="application-modal-field">
                            <label>Functie *</label>
                            <input
                                type="text"
                                name="functie"
                                value={formData.functie}
                                onChange={handleChange}
                            />
                        </div>

                        <div className="application-modal-field">
                            <label>Status *</label>
                            <input
                                type="text"
                                name="status"
                                value={formData.status}
                                onChange={handleChange}
                            />
                        </div>

                        <div className="application-modal-field">
                            <label>Datum *</label>
                            <input
                                type="text"
                                name="datum"
                                value={formData.datum}
                                onChange={handleChange}
                                placeholder="bijv. 15 mrt 2026"
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
