import ApplicationsTable from "../components/ApplicationsTable.tsx";
import { useState, type JSX } from "react";
import { Link } from "react-router-dom";
import ApplicationModal from "../components/ApplicationModal.tsx";
import ApplicationDetail from "../components/ApplicationsDetailComponent.tsx"
import { archiveApplication, getApplicationById } from "../services/applicationService.ts";
import type { DashboardOverviewItem } from "../types/dashboard.ts";
import type { ApplicationDetailResponse } from "../types/application.ts";

type ApplicationProps = {
    overview: DashboardOverviewItem[];
    setOverview: React.Dispatch<React.SetStateAction<DashboardOverviewItem[]>>;
}

export default function Application(props: ApplicationProps): JSX.Element {
    const [showModal, setShowModal] = useState<boolean>(false);
    const [showDetail, setShowDetail] = useState<boolean>(false);
    const [selectedApplication, setSelectedApplication] = useState<ApplicationDetailResponse | null>(null);

    function handleCreateClick() {
        setSelectedApplication(null);
        setShowModal(true);
    }

    async function handleEditClick(application: DashboardOverviewItem): Promise<void> {
        try {
            const detail: ApplicationDetailResponse = await getApplicationById(application.id);
            setSelectedApplication(detail);
            setShowModal(true);
        } catch (error) {
            console.error("Fout bij het ophalen van sollicitatie details:", error);
        }

    }
    async function handleDetailClick(application: DashboardOverviewItem): Promise<void> {
        try {
            const detail: ApplicationDetailResponse = await getApplicationById(application.id);
            setSelectedApplication(detail);
            setShowDetail(true);
        } catch (error) {
            console.error("Fout bij het ophalen van sollicitatie details:", error);
        }

    }

    async function handleArchiveClick(application: DashboardOverviewItem): Promise<void> {
        const confirmed = window.confirm(`Sollicitatie archiveren?\n\n${application.jobTitle} bij ${application.companyName}`);

        if (!confirmed) {
            return;
        }

        try {
            await archiveApplication(application.id);
            props.setOverview((prev: DashboardOverviewItem[]) =>
                prev.filter((item) => item.id !== application.id)
            );
        } catch (error) {
            window.alert(error instanceof Error ? error.message : "Fout bij het archiveren van de sollicitatie.");
        }
    }

    function handleCloseModal() {
        setShowModal(false);
        setSelectedApplication(null);
    }
    function handleCloseDetail() {
        setShowDetail(false);
        setSelectedApplication(null);
    }

    return (
        <section className="dashboard-container sollicitaties-page">
            <div className="page-header">
                <div className="page-heading">
                    <h1 className="dashboard-title">Sollicitaties</h1>
                    <p className="dashboard-subtitle">Beheer al je sollicitaties</p>
                </div>
                <div className="applications-page-actions">
                    <button type="button" className="new-application-button"
                        onClick={handleCreateClick}>
                        + Nieuwe sollicitatie
                    </button>
                    <Link className="applications-archive-link" to="/applications/archive">
                        Archief
                    </Link>
                </div>
            </div>
            <>
                <div className="applicationsTable-container">
                    <ApplicationsTable
                        key={props.overview.map((application) => application.id).join("-")}
                        applications={props.overview}
                        opSollicitatiePagina={true}
                        onEdit={handleEditClick}
                        onDetail={handleDetailClick}
                        onDelete={handleArchiveClick}
                    />
                </div>

                {showModal && <ApplicationModal
                    mode={selectedApplication ? "edit" : "create"}
                    initialApplication={selectedApplication as ApplicationDetailResponse | null}
                    onClose={handleCloseModal}
                    onCreated={(createdApplication: DashboardOverviewItem) => {
                        props.setOverview((prev: DashboardOverviewItem[]) => [createdApplication, ...prev]);
                    }}
                    onUpdated={(updatedApplication: DashboardOverviewItem) => {
                        props.setOverview((prev: DashboardOverviewItem[]) =>
                            prev.map((application) =>
                                application.id === updatedApplication.id ? updatedApplication : application
                            )
                        );
                    }}
                />}
                {showDetail && <ApplicationDetail
                    initialApplication={selectedApplication as ApplicationDetailResponse | null}
                    onClose={handleCloseDetail}
                />}
            </>
        </section>
    )
}
