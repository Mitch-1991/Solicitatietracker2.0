import { 
    LayoutDashboard,
    FileText,
    CalendarDays,
    Settings

} from "lucide-react";
import { NavLink } from "react-router-dom";

import type { LucideIcon } from "lucide-react";
import type {JSX} from "react";

type NavigationItem = {
    id: string;
    label: string;
    icon: LucideIcon;
    to: string;
};

const navigationItems: NavigationItem[] = [
    {
        id: "dashboard",
        label: "Dashboard",
        icon: LayoutDashboard,
        to: "/dashboard",
    },
    {
        id: "applications",
        label: "Sollicitaties",
        icon: FileText,
        to: "/applications",
    },
    {
        id: "calendar",
        label: "Kalender",
        icon: CalendarDays,
        to: "/calendar",
    },
    {
        id: "settings",
        label: "Instellingen",
        icon: Settings,
        to: "/settings",
    }
];

export default function SideBar(): JSX.Element {
    const navigationElements: JSX.Element[] = navigationItems.map((item: NavigationItem): JSX.Element => {
        const Icon: LucideIcon = item.icon;

        return (
            <li key={item.id} className="sidebar-nav-item">
                <NavLink
                    to={item.to}
                    className={({ isActive }) => `sidebar-nav-link ${isActive ? "active" : ""}`}
                >
                    <span className="sidebar-nav-icon">
                        <Icon />
                    </span>
                    <span className="sidebar-nav-label">{item.label}</span>
                </NavLink>
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
