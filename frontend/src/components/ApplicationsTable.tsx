import StatusBadge from "./StatusBadge"
import { useState, type JSX } from "react";
import { Eye, PencilLine, Trash2 } from "lucide-react";
import type { DashboardOverviewItem } from "../types/dashboard";

type DashboardTableProps = {
    applications: DashboardOverviewItem[];
    opSollicitatiePagina: false;
};

type ApplicationTableProps = {
    applications: DashboardOverviewItem[];
    opSollicitatiePagina: true;
    onDetail?: (application: DashboardOverviewItem) => void;
    onEdit?: (application: DashboardOverviewItem) => void;
    onDelete?: (application: DashboardOverviewItem) => void;
};

type ApplicationsTableProps = DashboardTableProps | ApplicationTableProps;
type TableAction = "Detail" | "Bewerken" | "Verwijderen";

export default function ApplicationsTable(props: ApplicationsTableProps) {

    const [currentPage, setCurrentPage] = useState<number>(1);

    const itemsPerPage = 5;
    const startIndex: number = (currentPage - 1) * itemsPerPage;
    const endIndex: number = startIndex + itemsPerPage;
    const currentItems: DashboardOverviewItem[] = props.applications.slice(startIndex, endIndex);
    const totalPages: number = Math.ceil(props.applications.length / itemsPerPage);
    const hasPagination: boolean = totalPages > 1;

    function handleActionClick(
        action: TableAction,
        application: DashboardOverviewItem
    ): void {
        console.log(`${action} sollicitatie`, application);

        if (!props.opSollicitatiePagina) return;

        switch (action) {
            case "Detail":
                props.onDetail?.(application);
                break;
            case "Bewerken":
                props.onEdit?.(application);
                break;
            case "Verwijderen":
                props.onDelete?.(application);
                break;
        }
    }

    const Sollicitatierij: JSX.Element[] = currentItems.map((app: DashboardOverviewItem): JSX.Element => {
        return (
            <tr key={app.id} className="applications-row">
                <td data-label="Bedrijf">{app.companyName}</td>
                <td data-label="Functie">{app.jobTitle}</td>
                <td data-label="Status"><StatusBadge status={app.status} /></td>
                <td data-label="Datum">{app.appliedDate}</td>
                <td data-label="Volgende stap">{app.nextStep}</td>
                {props.opSollicitatiePagina && (
                    <td data-label="Acties">
                        <div className="applications-actions">
                            <button
                                type="button"
                                className="applications-action-button"
                                aria-label={`Bekijk details van ${app.jobTitle} bij ${app.companyName}`}
                                title="Detail"
                                onClick={() => handleActionClick("Detail", app)}
                            >
                                <Eye size={20} strokeWidth={2.1} />
                            </button>
                            <button
                                type="button"
                                className="applications-action-button"
                                aria-label={`Bewerk ${app.jobTitle} bij ${app.companyName}`}
                                title="Bewerken"
                                onClick={() => handleActionClick("Bewerken", app)}
                            >
                                <PencilLine size={20} strokeWidth={2.1} />
                            </button>
                            <button
                                type="button"
                                className="applications-action-button"
                                aria-label={`Verwijder ${app.jobTitle} bij ${app.companyName}`}
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
