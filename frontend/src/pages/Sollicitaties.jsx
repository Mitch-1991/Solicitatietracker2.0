export default function Sollicitaties() {

    return (
        <section>
            <div className="applicationsTable-container">
                <h2 className="applicationsTable-title">Sollicitaties</h2>
                <p>Beheer al je sollicitaties</p>
                <ApplicationsTable applications={overview} opSollicitatiePagina={false} />
            </div>
        </section>
    )
}