import StatusBadge from "./StatudBadge"
import { useState, useEffect } from "react";

export default function ApplicationsTable(props) {

    const [currentPage, setCurrentPage] = useState(1);

    const itemsPerPage = 5;
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const currentItems = props.applications.slice(startIndex, endIndex);
    const totalPages = Math.ceil(props.applications.length / itemsPerPage);
    const hasPagination = totalPages > 1;

    const Sollicitatierij = currentItems.map((app) => {
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

    useEffect(() => {
        setCurrentPage(1);
    }, [props.applications]);

    return (
        <div>
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
        {hasPagination && (
            <div className="applications-pagination">
                <button
                    className="applications-pagination-button"
                    onClick={() => setCurrentPage(currentPage - 1)}
                    disabled={currentPage === 1}
                >
                    Vorige
                </button>
                <span className="applications-pagination-status">
                    Pagina {currentPage} van {totalPages}
                </span>
                <button
                    className="applications-pagination-button"
                    onClick={() => setCurrentPage(currentPage + 1)}
                    disabled={currentPage === totalPages}
                >
                    Volgende
                </button>
            </div>
        )}
        </div>

        
    )
}
