import { Briefcase, LogOut } from "lucide-react"
import type { JSX } from "react"
import { useNavigate } from "react-router-dom"
import { useAuth } from "../context/authContextValue"
import { EyeOff } from "lucide-react"

export default function Header(): JSX.Element {
  const { user, logout, isAuthenticated, isViewOnly } = useAuth()
  const navigate = useNavigate()

  const handleLogout = (): void => {
    logout()
    navigate("/login")
  }

  const initials = `${user?.firstName?.[0] ?? ""}${user?.lastName?.[0] ?? ""}`.toUpperCase()

  return (
    <header className="app-header">
      <div className="header-container">
        <div className="header-brand">
          <span className="header-logo" aria-hidden="true">
            <Briefcase />
          </span>
          <h1>SollicitatieTracker</h1>
        </div>

        {isAuthenticated && user && (
          <div className="header-user">
            <div className="header-user-text">
              <span className="header-user-name">
                {user.firstName} {user.lastName}
                {isViewOnly && (
                  <span className="header-view-only-badge">
                    <EyeOff size={11} aria-hidden="true" />
                    View Only
                  </span>
                )}
              </span>
              <span className="header-user-email">{user.email}</span>
            </div>
            <span className="header-avatar" aria-hidden="true">
              {initials}
            </span>
            <button className="header-logout-button" type="button" onClick={handleLogout}>
              <LogOut aria-hidden="true" />
              <span>Logout</span>
            </button>
          </div>
        )}
      </div>
    </header>
  )
}
