import { Briefcase } from "lucide-react"
import type { JSX } from "react"
export default function Header(): JSX.Element {
    return(
        <div className="header-container">
            <span className="header-logo">
                <Briefcase />
            </span>
            <h1>SollicitatieTracker</h1>
        </div>
        
    )
}
