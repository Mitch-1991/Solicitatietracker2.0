import {
    FileText,
    CheckSquare,
    Calendar,
    Clock,
} from "lucide-react";
export default function Body() {

    return (
        <main>
            <div className="kpi-container">
                <div className="kpi-card">
                    <div>
                        <p className="kpi-label">Lopende sollicitatie</p>
                        <p className="kpi-waarde">12</p>
                    </div>
                    <div className="kpi-icon">
                        <FileText />
                    </div>

                </div>
                <div className="kpi-card">
                    <div>
                        <p className="kpi-label">Lopende sollicitatie</p>
                        <p className="kpi-waarde">12</p>
                    </div>
                    <div className="kpi-icon">
                        <FileText />
                    </div>

                </div>
                <div className="kpi-card">
                    <div>
                        <p className="kpi-label">Lopende sollicitatie</p>
                        <p className="kpi-waarde">12</p>
                    </div>
                    <div className="kpi-icon">
                        <FileText />
                    </div>

                </div>
                <div className="kpi-card">
                    <div>
                        <p className="kpi-label">Lopende sollicitatie</p>
                        <p className="kpi-waarde">12</p>
                    </div>
                    <div className="kpi-icon">
                        <FileText />
                    </div>
                </div>
            </div>
        </main>
    )
}


// importeren van de KPI info om de KPI components op te maken