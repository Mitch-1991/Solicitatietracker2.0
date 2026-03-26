
import { useState, useEffect } from "react";
import { getDashboardKpis } from "../Services/dashboardService";
import { MapKPIs } from "../mappers/kpiMapper";
import StatCard from "./StatCard";
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
            label= {kpi.label}
            waarde= {kpi.value}
            icoon = {kpi.icon}
            kleur = {kpi.color}
            />)
    return (
        <main className="dashboard-container">
            <h1>Dashboard</h1>
            <p>Overzicht van je lopende sollicitaties</p>
            <div className="kpi-container">
                {KPIElements}
            </div>
        </main>
    )
}
