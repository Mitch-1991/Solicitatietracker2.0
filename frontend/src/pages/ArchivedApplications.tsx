import { Archive, Eye } from "lucide-react";
import { useEffect, useState, type JSX } from "react";
import { Link } from "react-router-dom";
import ApplicationDetail from "../components/ApplicationsDetailComponent";
import { getArchivedApplicationById, getArchivedApplications } from "../services/applicationService";
import type { ApplicationDetailResponse } from "../types/application";

export default function ArchivedApplications(): JSX.Element {
    const [applications, setApplications] = useState<ApplicationDetailResponse[]>([]);
    const [selectedApplication, setSelectedApplication] = useState<ApplicationDetailResponse | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        async function fetchArchivedApplications(): Promise<void> {
            try {
                setIsLoading(true);
                setError(null);
                const data = await getArchivedApplications();
                setApplications(data);
            } catch (err) {
                setError(err instanceof Error ? err.message : "Fout bij het ophalen van het archief.");
            } finally {
                setIsLoading(false);
            }
        }

        fetchArchivedApplications();
    }, []);

    async function handleDetailClick(applicationId: number): Promise<void> {
        try {
            const detail = await getArchivedApplicationById(applicationId);
            setSelectedApplication(detail);
        } catch (err) {
            setError(err instanceof Error ? err.message : "Fout bij het ophalen van de gearchiveerde sollicitatie.");
        }
    }

    return (
        <section className="dashboard-container sollicitaties-page">
            <div className="page-header">
                <div className="page-heading">
                    <h1 className="dashboard-title">Archief</h1>
                    <p className="dashboard-subtitle">Gearchiveerde sollicitaties blijven hier beschikbaar als historiek.</p>
                </div>
                <Link className="applications-archive-link archive-back-link" to="/applications">
                    Terug naar sollicitaties
                </Link>
            </div>

            <div className="applicationsTable-container">
                <h2 className="applicationsTable-title">Gearchiveerde sollicitaties</h2>

                {isLoading && <p className="archive-state">Archief laden...</p>}
                {error && <p className="archive-state archive-state-error">{error}</p>}
                {!isLoading && !error && applications.length === 0 && (
                    <div className="archive-empty-state">
                        <Archive aria-hidden="true" />
                        <p>Geen gearchiveerde sollicitaties.</p>
                    </div>
                )}

                {!isLoading && applications.length > 0 && (
                    <table className="applications-table archive-table">
                        <thead>
                            <tr>
                                <th>Bedrijf</th>
                                <th>Functie</th>
                                <th>Status</th>
                                <th>Datum</th>
                                <th>Gearchiveerd op</th>
                                <th>Acties</th>
                            </tr>
                        </thead>
                        <tbody>
                            {applications.map((application) => (
                                <tr key={application.id} className="applications-row">
                                    <td data-label="Bedrijf">{application.companyName}</td>
                                    <td data-label="Functie">{application.jobTitle}</td>
                                    <td data-label="Status">{application.status}</td>
                                    <td data-label="Datum">{application.appliedDate ?? "-"}</td>
                                    <td data-label="Gearchiveerd op">{application.archivedAt ? formatDateTime(application.archivedAt) : "-"}</td>
                                    <td data-label="Acties">
                                        <button
                                            type="button"
                                            className="applications-action-button"
                                            aria-label={`Bekijk details van ${application.jobTitle} bij ${application.companyName}`}
                                            title="Detail"
                                            onClick={() => handleDetailClick(application.id)}
                                        >
                                            <Eye size={20} strokeWidth={2.1} />
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>

            {selectedApplication && (
                <ApplicationDetail
                    initialApplication={selectedApplication}
                    onClose={() => setSelectedApplication(null)}
                />
            )}
        </section>
    );
}

function formatDateTime(value: string): string {
    return new Intl.DateTimeFormat("nl-BE", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit"
    }).format(new Date(value));
}
