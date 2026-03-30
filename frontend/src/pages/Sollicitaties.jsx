import ApplicationsTable from "../Components/ApplicationsTable.jsx";

export default function Sollicitaties(props) {

    return (
        <section className="dashboard-container sollicitaties-page">
            <div className="page-header">
                <div className="page-heading">
                    <h1 className="dashboard-title">Sollicitaties</h1>
                    <p className="dashboard-subtitle">Beheer al je sollicitaties</p>
                </div>
                <button type="button" className="new-application-button">
                    + Nieuwe sollicitatie
                </button>
            </div>
            <div className="applicationsTable-container">
                <ApplicationsTable applications={props.overview} opSollicitatiePagina={true} />
            </div>
        </section>
    )
}
