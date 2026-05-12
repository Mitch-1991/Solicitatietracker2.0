import type { LucideIcon } from "lucide-react";
import type { JSX } from "react";

type StatCardProps = {
    label: string;
    value: number;
    icon: LucideIcon;
    color: string;
}

export default function StatCard({ label, value, icon: Icon, color }: StatCardProps): JSX.Element {
    return (
        <div className="kpi-card">
            <div>
                <p className="kpi-label">{label}</p>
                <p className="kpi-waarde">{value}</p>
            </div>
            <div className="kpi-icon" style={{ background: color }} aria-hidden="true">
                <Icon />
            </div>
        </div>
    )
}
