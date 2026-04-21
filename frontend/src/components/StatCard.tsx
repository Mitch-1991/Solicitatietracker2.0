import type { MappedKpi } from "../types/dashboard";
import type { LucideIcon } from "lucide-react";
import type { JSX } from "react";

type StatCardProps = {
    label: string;
    value: number;
    icon: LucideIcon;
    color: string;
}

export default function StatCard(props: StatCardProps): JSX.Element {

    const Icon: LucideIcon = props.icon
    const styles= {
        backgroundColor: props.color
    }
    return (
        <div className="kpi-card" >
            <div>
                <p className="kpi-label">{props.label}</p>
                <p className="kpi-waarde">{props.value}</p>
            </div>
            <div className="kpi-icon" style={styles}>
                <Icon />
            </div>

        </div>
    )
}
