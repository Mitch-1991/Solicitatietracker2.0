import { 
    LayoutDashboard,
    FileText

} from "lucide-react";

const navigationItems = [
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

export default function SideBar() {
    const navigationElements = navigationItems.map((item) => {
        const Icon = item.icon;

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
