import ApplicationsTable from "../Components/ApplicationsTable.jsx";
import { useState } from "react";
import ApplicationModal from "../Components/ApplicationModal.jsx";

export default function Sollicitaties(props) {
    const [showModal, setShowModal] = useState(false);

    function renderContent() {
        if (showModal) {
            return <ApplicationModal onClose={() => setShowModal(false)} />
        }
        return (
        <div className="applicationsTable-container">
            <ApplicationsTable applications={props.overview} opSollicitatiePagina={true} />
        </div>)
    }

    return (
        <section className="dashboard-container sollicitaties-page">
            <div className="page-header">
                <div className="page-heading">
                    <h1 className="dashboard-title">Sollicitaties</h1>
                    <p className="dashboard-subtitle">Beheer al je sollicitaties</p>
                </div>
                <button type="button" className="new-application-button"
                    onClick={() => setShowModal(true)}>
                    + Nieuwe sollicitatie
                </button>
            </div>
            {renderContent()}
        </section>
    )
}
