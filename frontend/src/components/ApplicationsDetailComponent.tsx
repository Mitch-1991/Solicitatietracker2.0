import { mapApplicationToFormData } from "../mappers/applicationMappers.ts";
import type {ApplicationFormData, ApplicationDetailResponse} from "../types/application.ts";

type ApplicationDetailProps = {
    initialApplication: ApplicationDetailResponse | null;
    onClose: () => void;
};
export default function ApplicationDetail(props: ApplicationDetailProps) {

    const formData: ApplicationFormData = mapApplicationToFormData(props.initialApplication);
    const interview = props.initialApplication?.interview;
    const title = props.initialApplication?.isArchived ? "Gearchiveerde sollicitatie" : "Sollicitatiedetails";
    const archivedAt = props.initialApplication?.archivedAt ? formatDateTime(props.initialApplication.archivedAt) : null;

  
    function handleClose(): void {
        props.onClose();
    }

    return (
        <section
            className="application-modal-overlay"
            onClick={(e) => e.target === e.currentTarget && handleClose()}
        >
            <div className="application-modal">
                <div className="application-modal-header">
                    <h2 className="application-modal-title">{title}</h2>
                    <button
                        type="button"
                        className="application-modal-close"
                        onClick={handleClose}
                        aria-label="Sluit modal"
                    >
                        x
                    </button>
                </div>

                <div className="application-modal-form">
                    <div className="application-detail-grid">
                        <p><strong>Bedrijf:</strong> {formData.companyName}</p>
                        <p><strong>Jobtitel:</strong> {formData.jobTitle}</p>
                        <p><strong>Status:</strong> {formData.status}</p>
                        <p><strong>Sollicitatiedatum:</strong> {formData.date || "-"}</p>
                        <p><strong>Prioriteit:</strong> {formData.priority || "-"}</p>
                        <p><strong>Locatie:</strong> {formData.location || "-"}</p>
                        <p><strong>Job URL:</strong> {formData.jobUrl || "-"}</p>
                        <p><strong>Volgende stap:</strong> {formData.nextStep || "-"}</p>
                        <p><strong>Beschrijving:</strong> {formData.notes || "-"}</p>
                        {archivedAt && <p><strong>Gearchiveerd op:</strong> {archivedAt}</p>}
                    </div>

                    {interview && (
                        <div className="application-detail-section">
                            <h3 className="application-modal-section-title">Interview</h3>
                            <div className="application-detail-grid">
                                <p><strong>Type:</strong> {interview.interviewType || "-"}</p>
                                <p><strong>Datum:</strong> {formatDate(interview.scheduledStart)}</p>
                                <p><strong>Startuur:</strong> {formatTime(interview.scheduledStart)}</p>
                                <p><strong>Einduur:</strong> {interview.scheduledEnd ? formatTime(interview.scheduledEnd) : "-"}</p>
                                <p><strong>Locatie:</strong> {interview.location || "-"}</p>
                                <p><strong>Meeting link:</strong> {interview.meetingLink ? (
                                    <a href={interview.meetingLink} target="_blank" rel="noreferrer">{interview.meetingLink}</a>
                                ) : "-"}</p>
                                <p><strong>Contactpersoon:</strong> {interview.contactPerson || "-"}</p>
                                <p><strong>Contact e-mail:</strong> {interview.contactEmail || "-"}</p>
                                <p><strong>Notities:</strong> {interview.notes || "-"}</p>
                            </div>
                        </div>
                    )}

                    <div className="application-modal-grid">
                        <div className="application-modal-footer">
                            <button
                                type="button"
                                className="application-modal-button application-modal-button-primary"
                                onClick={handleClose}
                            >
                                Sluiten
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </section>

    )
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

function formatDate(value: string): string {
    return new Intl.DateTimeFormat("nl-BE", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric"
    }).format(new Date(value));
}

function formatTime(value: string): string {
    return new Intl.DateTimeFormat("nl-BE", {
        hour: "2-digit",
        minute: "2-digit"
    }).format(new Date(value));
}
