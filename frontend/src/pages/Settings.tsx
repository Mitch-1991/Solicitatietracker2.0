import { Lock, UserRound } from "lucide-react"
import { useState } from "react"
import { useAuth } from "../context/AuthContext"
import { changePassword } from "../services/authService"

export default function Settings() {
  const { user } = useAuth()
  const [currentPassword, setCurrentPassword] = useState("")
  const [newPassword, setNewPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const [message, setMessage] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault()
    setIsSubmitting(true)
    setMessage(null)
    setError(null)

    if (newPassword !== confirmPassword) {
      setError("Wachtwoorden komen niet overeen.")
      setIsSubmitting(false)
      return
    }

    try {
      await changePassword({ currentPassword, newPassword, confirmPassword })
      setMessage("Je wachtwoord is gewijzigd.")
      setCurrentPassword("")
      setNewPassword("")
      setConfirmPassword("")
    } catch (err) {
      setError(err instanceof Error ? err.message : "Wachtwoord wijzigen mislukt.")
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="dashboard-container settings-page">
      <header className="page-header">
        <div className="page-heading">
          <h1 className="dashboard-title">Instellingen</h1>
          <p className="dashboard-subtitle">Beheer je account en wachtwoord.</p>
        </div>
      </header>

      <section className="settings-grid">
        <article className="settings-panel">
          <div className="settings-panel-header">
            <UserRound aria-hidden="true" />
            <h2>Account</h2>
          </div>
          <dl className="settings-details">
            <div>
              <dt>Naam</dt>
              <dd>{user ? `${user.firstName} ${user.lastName}` : "-"}</dd>
            </div>
            <div>
              <dt>E-mailadres</dt>
              <dd>{user?.email ?? "-"}</dd>
            </div>
          </dl>
        </article>

        <article className="settings-panel">
          <div className="settings-panel-header">
            <Lock aria-hidden="true" />
            <h2>Wachtwoord wijzigen</h2>
          </div>

          <form className="settings-form" onSubmit={handleSubmit}>
            <label className="settings-field">
              <span>Huidig wachtwoord</span>
              <input
                type="password"
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
                autoComplete="current-password"
                required
              />
            </label>

            <label className="settings-field">
              <span>Nieuw wachtwoord</span>
              <input
                type="password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                autoComplete="new-password"
                minLength={6}
                required
              />
            </label>

            <label className="settings-field">
              <span>Bevestig wachtwoord</span>
              <input
                type="password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                autoComplete="new-password"
                minLength={6}
                required
              />
            </label>

            {message && <p className="settings-success">{message}</p>}
            {error && <p className="settings-error">{error}</p>}

            <button className="settings-submit" type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Bezig..." : "Wachtwoord wijzigen"}
            </button>
          </form>
        </article>
      </section>
    </main>
  )
}
