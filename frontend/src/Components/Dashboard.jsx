
import { useState, useEffect } from "react";
import { getDashboardKpis } from "../Services/dashboardService";
import { MapKPIs } from "../mappers/kpiMapper";
import StatCard from "./StatCard";
import ApplicationsTable from "./ApplicationsTable";
import { Solicitaties } from "../dummy";
export default function Dashboard() {
    const [kpis, setKpis] = useState([])

    useEffect(() => {
        const fetchKpis = async () => {
            const data = await getDashboardKpis()
            console.log(data)
            const mappedKpis = MapKPIs(data)
            setKpis(mappedKpis)
        };
        fetchKpis()
    }, [])

    const KPIElements = kpis.map((kpi) =>
        <StatCard key={kpi.id}
            label={kpi.label}
            waarde={kpi.value}
            icoon={kpi.icon}
            kleur={kpi.color}
        />)
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
                    <ApplicationsTable applications={Solicitaties} />
                </div>
            </div>
        </main>
    )
}
