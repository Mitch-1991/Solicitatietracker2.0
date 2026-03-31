import StatusBadge from "./StatudBadge"
import { useState } from "react";
import { Eye, PencilLine, Trash2 } from "lucide-react";

export default function ApplicationsTable(props) {

    const [currentPage, setCurrentPage] = useState(1);

    const itemsPerPage = 5;
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const currentItems = props.applications.slice(startIndex, endIndex);
    const totalPages = Math.ceil(props.applications.length / itemsPerPage);
    const hasPagination = totalPages > 1;

    function handleActionClick(action, application) {
        console.log(`${action} sollicitatie`, application);

        if (action === "Detail" && props.onDetail) {
            props.onDetail(application);
        }

        if (action === "Bewerken" && props.onEdit) {
            props.onEdit(application);
        }

        if (action === "Verwijderen" && props.onDelete) {
            props.onDelete(application);
        }
    }

    const Sollicitatierij = currentItems.map((app) => {
        return (
            <tr key={app.id} className="applications-row">
                <td data-label="Bedrijf">{app.bedrijf}</td>
                <td data-label="Functie">{app.functie}</td>
                <td data-label="Status"><StatusBadge status={app.status} /></td>
                <td data-label="Datum">{app.datum}</td>
                <td data-label="Volgende stap">{app.volgendeStap}</td>
                {props.opSollicitatiePagina && (
                    <td data-label="Acties">
                        <div className="applications-actions">
                            <button
                                type="button"
                                className="applications-action-button"
                                aria-label={`Bekijk details van ${app.functie} bij ${app.bedrijf}`}
                                title="Detail"
                                onClick={() => handleActionClick("Detail", app)}
                            >
                                <Eye size={20} strokeWidth={2.1} />
                            </button>
                            <button
                                type="button"
                                className="applications-action-button"
                                aria-label={`Bewerk ${app.functie} bij ${app.bedrijf}`}
                                title="Bewerken"
                                onClick={() => handleActionClick("Bewerken", app)}
                            >
                                <PencilLine size={20} strokeWidth={2.1} />
                            </button>
                            <button
                                type="button"
                                className="applications-action-button"
                                aria-label={`Verwijder ${app.functie} bij ${app.bedrijf}`}
                                title="Verwijderen"
                                onClick={() => handleActionClick("Verwijderen", app)}
                            >
                                <Trash2 size={20} strokeWidth={2.1} />
                            </button>
                        </div>
                    </td>
                )}
            </tr>
        )

    })
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
                    {props.opSollicitatiePagina && <th>Acties</th>}
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
