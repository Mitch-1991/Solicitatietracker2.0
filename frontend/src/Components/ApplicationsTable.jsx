import StatusBadge from "./StatudBadge"

export default function ApplicationsTable(props) {
    const Sollicitatierij = props.applications.map((app) => {
        return (
            <tr key={app.id} className="applications-row">
                <td>{app.bedrijf}</td>
                <td>{app.functie}</td>
                <td><StatusBadge status={app.status} /></td>
                <td>{app.datum}</td>
                <td>{app.volgendeStap}</td>
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
