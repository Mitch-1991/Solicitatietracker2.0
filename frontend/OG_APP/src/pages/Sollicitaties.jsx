import ApplicationsTable from "../Components/ApplicationsTable.jsx";
import { useState } from "react";
import ApplicationModal from "../Components/ApplicationModal.jsx";
import ApplicationDetail from "../Components/ApplicationDetailComponent.jsx"

export default function Sollicitaties(props) {
    const [showModal, setShowModal] = useState(false);
    const [showDetail, setShowDetail] = useState(false);
    const [selectedApplication, setSelectedApplication] = useState(null);

    function handleCreateClick() {
        setSelectedApplication(null);
        setShowModal(true);
    }

    function handleEditClick(application) {
        setSelectedApplication(application);
        setShowModal(true);
    }
    function handleDetailClick(application){
        setSelectedApplication(application);
        setShowDetail(true);
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
                <button type="button" className="new-application-button"
                    onClick={handleCreateClick}>
                    + Nieuwe sollicitatie
                </button>
            </div>
            <>
                <div className="applicationsTable-container">
                    <ApplicationsTable
                        key={props.overview.map((application) => application.id).join("-")}
                        applications={props.overview}
                        opSollicitatiePagina={true}
                        onEdit={handleEditClick}
                        onDetail={handleDetailClick}
                    />
                </div>

                {showModal && <ApplicationModal
                    mode={selectedApplication ? "edit" : "create"}
                    initialApplication={selectedApplication}
                    onClose={handleCloseModal}
                    onCreated={(createdApplication) => {props.setOverview((prev) => [createdApplication, ...prev]);
                    }}
                    onUpdated={(updatedApplication) => {
                        props.setOverview((prev) =>
                            prev.map((application) =>
                                application.id === updatedApplication.id ? updatedApplication : application
                            )
                        );
                    }}
                />}
                {showDetail && <ApplicationDetail 
                initialApplication={selectedApplication}
                onClose={handleCloseDetail}
                />}
            </>
        </section>
    )
}
