import ApplicationsTable from "../Components/ApplicationsTable.jsx";

export default function Sollicitaties(props) {

    return (
        <section>
            <div className="applicationsTable-container">
                <h2 className="applicationsTable-title">Sollicitaties</h2>
                <ApplicationsTable applications={props.overview} opSollicitatiePagina={true} />
            </div>
        </section>
    )
}