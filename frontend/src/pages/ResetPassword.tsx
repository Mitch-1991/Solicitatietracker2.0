import { Briefcase, Lock } from "lucide-react"
import { useState } from "react"
import { Link, useSearchParams } from "react-router-dom"
import { resetPassword } from "../services/authService"

export default function ResetPassword() {
  const [searchParams] = useSearchParams()
  const token = searchParams.get("token") ?? ""
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
      await resetPassword({ token, newPassword, confirmPassword })
      setMessage("Je wachtwoord is gewijzigd. Je kan nu inloggen.")
      setNewPassword("")
      setConfirmPassword("")
    } catch (err) {
      setError(err instanceof Error ? err.message : "Wachtwoord resetten mislukt.")
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-shell" aria-labelledby="reset-password-title">
        <div className="auth-brand">
          <span className="auth-logo" aria-hidden="true">
            <Briefcase />
          </span>
          <h1>SollicitatieTracker</h1>
          <p>Kies een nieuw wachtwoord voor je account</p>
        </div>

        <div className="auth-card">
          <h2 id="reset-password-title">Nieuw wachtwoord</h2>

          <form className="auth-form" onSubmit={handleSubmit}>
            <label className="auth-field">
              <span>Nieuw wachtwoord</span>
              <span className="auth-input-wrap">
                <Lock aria-hidden="true" />
                <input
                  type="password"
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  autoComplete="new-password"
                  minLength={6}
                  required
                />
              </span>
            </label>

            <label className="auth-field">
              <span>Bevestig wachtwoord</span>
              <span className="auth-input-wrap">
                <Lock aria-hidden="true" />
                <input
                  type="password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  autoComplete="new-password"
                  minLength={6}
                  required
                />
              </span>
            </label>

            {!token && <p className="auth-error">Reset-token ontbreekt.</p>}
            {message && <p className="auth-success">{message}</p>}
            {error && <p className="auth-error">{error}</p>}

            <button className="auth-submit" type="submit" disabled={isSubmitting || !token}>
              {isSubmitting ? "Bezig..." : "Wachtwoord wijzigen"}
            </button>
          </form>

          <p className="auth-switch">
            Klaar? <Link to="/login">Terug naar login</Link>
          </p>
        </div>
      </section>
    </main>
  )
}
