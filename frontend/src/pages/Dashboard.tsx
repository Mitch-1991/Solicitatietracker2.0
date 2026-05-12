import { useState, useEffect, type JSX } from "react";
import { getDashboardKpis, getUpcomingInterviews } from "../services/dashboardService.ts";
import { MapKPIs, MapUpcomingInterviews } from "../mappers/dashboardMappers.ts";
import StatCard from "../components/StatCard.tsx";
import ApplicationsTable from "../components/ApplicationsTable.tsx";
import type {
    DashboardOverviewItem,
    MappedKpi,
    DashboardKpiResponse,
    UpcomingInterview,
    UpcomingInterviewResponse
} from "../types/dashboard.ts";

export default function Dashboard({overview}: {overview: DashboardOverviewItem[]}): JSX.Element {
    const [kpis, setKpis] = useState<MappedKpi[]>([])
    const [upcomingInterviews, setUpcomingInterviews] = useState<UpcomingInterview[]>([])
    const [isLoading, setIsLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        let cancelled = false

        const fetchData = async (): Promise<void> => {
            try {
                const [kpiData, interviewData]: [DashboardKpiResponse, UpcomingInterviewResponse[]] = await Promise.all([
                    getDashboardKpis(),
                    getUpcomingInterviews()
                ])
                if (cancelled) return
                setKpis(MapKPIs(kpiData))
                setUpcomingInterviews(MapUpcomingInterviews(interviewData))
            } catch {
                if (!cancelled) setError("Dashboard kon niet worden geladen. Probeer de pagina te vernieuwen.")
            } finally {
                if (!cancelled) setIsLoading(false)
            }
        }

        fetchData()
        return () => { cancelled = true }
    }, [])

    const KPIElements: JSX.Element[] = kpis.map((kpi: MappedKpi): JSX.Element =>
        <StatCard key={kpi.id}
            label={kpi.label}
            value={kpi.value}
            icon={kpi.icon}
            color={kpi.color}
        />)

    const InterviewElements: JSX.Element[] = upcomingInterviews.map((interview: UpcomingInterview): JSX.Element => {
        return (
            <li
                key={`${interview.id}-${interview.interviewDate}-${interview.hour}`}
                className="upcoming-interview-card"
            >
                <h3 className="upcoming-interview-company">{interview.companyName}</h3>
                <p className="upcoming-interview-role">{interview.jobTitle}</p>
                <div className="upcoming-interview-detail">
                    <span className="upcoming-interview-detail-label">Datum</span>
                    <span className="upcoming-interview-detail-value">{interview.interviewDate}</span>
                </div>
                <div className="upcoming-interview-detail">
                    <span className="upcoming-interview-detail-label">Tijd</span>
                    <span className="upcoming-interview-detail-value">{interview.hour}</span>
                </div>
            </li>
        )
    })
    return (
        <section className="dashboard-container">
            <h1 className="dashboard-title">Dashboard</h1>
            <p className="dashboard-subtitle">Overzicht van je lopende sollicitaties</p>
            {error && <p className="dashboard-error" role="alert">{error}</p>}
            <div className="kpi-container">
                {isLoading
                    ? Array.from({ length: 4 }, (_, i) => <div key={i} className="kpi-skeleton" aria-hidden="true" />)
                    : KPIElements
                }
            </div>
            <div className="display-bord">
                <div className="applicationsTable-container">
                    <h2 className="applicationsTable-title">Lopende sollicitaties</h2>
                    <ApplicationsTable applications={overview} opSollicitatiePagina={false} />
                </div>
                <aside className="upcoming-interviews-panel">
                    <h2 className="upcoming-interviews-title">Komende gesprekken</h2>
                    <ul className="upcoming-interviews-list">
                        {upcomingInterviews.length > 0
                            ? InterviewElements
                            : <li className="upcoming-interviews-empty">Geen aankomende gesprekken</li>
                        }
                    </ul>
                </aside>
            </div>
        </section>
    )
}
