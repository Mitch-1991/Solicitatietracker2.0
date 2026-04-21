import { 
    LayoutDashboard,
    FileText

} from "lucide-react";
import type { LucideIcon } from "lucide-react";
import type {JSX} from "react";

type NavigationItem = {
    id: string;
    label: string;
    icon: LucideIcon;
    active: boolean;
};

const navigationItems: NavigationItem[] = [
    {
        id: "dashboard",
        label: "Dashboard",
        icon: LayoutDashboard,
        active: true,
    },
    {
        id: "sollicitaties",
        label: "Sollicitaties",
        icon: FileText,
        active: true,
    }
];

export default function SideBar(): JSX.Element {
    const navigationElements: JSX.Element[] = navigationItems.map((item: NavigationItem): JSX.Element => {
        const Icon: LucideIcon = item.icon;

        return (
            <li key={item.id} className="sidebar-nav-item">
                <button
                    type="button"
                    className={`sidebar-nav-link${item.active ? " active" : ""}`}
                >
                    <span className="sidebar-nav-icon">
                        <Icon />
                    </span>
                    <span className="sidebar-nav-label">{item.label}</span>
                </button>
            </li>
        );
    });

    return (
        <aside className="sidebar-container">
            <nav className="sidebar-nav" aria-label="Hoofdnavigatie">
                <ul className="sidebar-nav-list">
                    {navigationElements}
                </ul>
            </nav>
        </aside>
    );
}
