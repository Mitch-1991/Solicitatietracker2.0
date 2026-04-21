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

    useEffect(() => {
        const fetchKpis = async (): Promise<void> => {
            const data: DashboardKpiResponse = await getDashboardKpis()
            const mappedKpis: MappedKpi[] = MapKPIs(data)
            setKpis(mappedKpis)
        };

        const fetchUpcomingInterviews = async (): Promise<void> => {
            const data: UpcomingInterviewResponse[] = await getUpcomingInterviews()
            const mappedInterviews: UpcomingInterview[] = MapUpcomingInterviews(data)
            setUpcomingInterviews(mappedInterviews)
        }

        fetchKpis()
        fetchUpcomingInterviews()
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
            <div className="kpi-container">
                {KPIElements}
            </div>
            <div className="display-bord">
                <div className="applicationsTable-container">
                    <h2 className="applicationsTable-title">Lopende sollicitaties</h2>
                    <ApplicationsTable applications={overview} opSollicitatiePagina={false} />
                </div>
                <aside className="upcoming-interviews-panel">
                    <h2 className="upcoming-interviews-title">Komende gesprekken</h2>
                    <ul className="upcoming-interviews-list">
                        {upcomingInterviews.length > 0 ? InterviewElements : <p>Geen aankomende gesprekken</p>}
                    </ul>
                </aside>
            </div>
        </section>
    )
}
