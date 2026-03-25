import {
    FileText,
    CheckSquare,
    Calendar,
    Clock,
} from "lucide-react";
import { KPIs } from "../KPI";
import StatCard from "./StatCard";
export default function Body() {

    const KPIElements = KPIs.map((kpi) => 
            <StatCard 
            label= {kpi.label}
            waarde= {kpi.value}
            icoon = {kpi.icon}
            kleur = {kpi.color}
            />)
    return (
        <main>
            <div className="kpi-container">
                {KPIElements}
            </div>
        </main>
    )
}


// importeren van de KPI info om de KPI components op te maken