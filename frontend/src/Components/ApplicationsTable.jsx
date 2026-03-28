import StatusBadge from "./StatudBadge"

export default function ApplicationsTable(props) {
    const Sollicitatierij = props.applications.map((app) => {
        return (
            <tr key={app.id} className="applications-row">
                <td data-label="Bedrijf">{app.bedrijf}</td>
                <td data-label="Functie">{app.functie}</td>
                <td data-label="Status"><StatusBadge status={app.status} /></td>
                <td data-label="Datum">{app.datum}</td>
                <td data-label="Volgende stap">{app.volgendeStap}</td>
            </tr>
        )

    })

    return (
        <table className="applications-table">
            <thead>
                <tr>
                    <th>Bedrijf</th>
                    <th>Functie</th>
                    <th>Status</th>
                    <th>Datum</th>
                    <th>Volgende Stap</th>
                </tr>
            </thead>
            <tbody>
                {Sollicitatierij}
            </tbody>
        </table>
    )
}
