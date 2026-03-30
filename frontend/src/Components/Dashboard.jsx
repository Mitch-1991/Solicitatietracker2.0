
import { useState, useEffect } from "react";
import { getDashboardKpis, getDashboardOverview, getUpcomingInterviews  } from "../Services/dashboardService";
import { MapKPIs } from "../mappers/kpiMapper";
import { MapOverview } from "../mappers/overviewMapper";
import { MapUpcomingInterviews } from "../mappers/UpcomingInterviewMapper.js";
import StatCard from "./StatCard";
import ApplicationsTable from "./ApplicationsTable";

export default function Dashboard() {
    const [kpis, setKpis] = useState([])
    const [overview, setOverview] = useState([])
    const [upcomingInterviews, setUpcomingInterviews] = useState([])

    useEffect(() => {
        const fetchKpis = async () => {
            const data = await getDashboardKpis()
            const mappedKpis = MapKPIs(data)
            setKpis(mappedKpis)
        };
        const fetchOverview = async () => {
            const data = await getDashboardOverview()
            const mappedOverview = MapOverview(data)
            setOverview(mappedOverview)
        }
        const fetchUpcomingInterviews = async () => {
            const data = await getUpcomingInterviews()
            console.log(data)
            const mappedInterviews = MapUpcomingInterviews(data)
            setUpcomingInterviews(mappedInterviews)
        }


        fetchKpis()
        fetchOverview()
        fetchUpcomingInterviews()
    }, [])

    const KPIElements = kpis.map((kpi) =>
        <StatCard key={kpi.id}
            label={kpi.label}
            waarde={kpi.value}
            icoon={kpi.icon}
            kleur={kpi.color}
        />)
    const InterviewElements = upcomingInterviews.map((interview) => {
        return (
            <li
                key={`${interview.id}-${interview.datum}-${interview.uur}`}
                className="upcoming-interview-card"
            >
                <h3 className="upcoming-interview-company">{interview.bedrijf}</h3>
                <p className="upcoming-interview-role">{interview.functie}</p>
                <div className="upcoming-interview-detail">
                    <span className="upcoming-interview-detail-label">Datum</span>
                    <span className="upcoming-interview-detail-value">{interview.datum}</span>
                </div>
                <div className="upcoming-interview-detail">
                    <span className="upcoming-interview-detail-label">Tijd</span>
                    <span className="upcoming-interview-detail-value">{interview.uur}</span>
                </div>
            </li>
        )
    })
    return (
        <main className="dashboard-container">
            <h1 className="dashboard-title">Dashboard</h1>
            <p className="dashboard-subtitle">Overzicht van je lopende sollicitaties</p>
            <div className="kpi-container">
                {KPIElements}
            </div>
            <div className="display-bord">
                <div className="applicationsTable-container">
                    <h2 className="applicationsTable-title">Lopende sollicitaties</h2>
                    <ApplicationsTable applications={overview} />
                </div>
                <aside className="upcoming-interviews-panel">
                    <h2 className="upcoming-interviews-title">Komende gesprekken</h2>
                    <ul className="upcoming-interviews-list">
                        {upcomingInterviews.length > 0 ? InterviewElements : <p>Geen aankomende gesprekken</p>}
                    </ul>
                </aside>
            </div>
        </main>
    )
}
