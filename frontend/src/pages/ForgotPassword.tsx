import { Briefcase, Mail } from "lucide-react"
import { useState } from "react"
import { Link } from "react-router-dom"
import { forgotPassword } from "../services/authService"

export default function ForgotPassword() {
  const [email, setEmail] = useState("")
  const [message, setMessage] = useState<string | null>(null)
  const [devResetUrl, setDevResetUrl] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault()
    setIsSubmitting(true)
    setMessage(null)
    setDevResetUrl(null)
    setError(null)

    try {
      const response = await forgotPassword({ email })
      setMessage(response.message)
      setDevResetUrl(response.resetUrl ?? null)
    } catch (err) {
      setError(err instanceof Error ? err.message : "Resetlink aanvragen mislukt.")
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-shell" aria-labelledby="forgot-password-title">
        <div className="auth-brand">
          <span className="auth-logo" aria-hidden="true">
            <Briefcase />
          </span>
          <h1>SollicitatieTracker</h1>
          <p>Vraag een link aan om je wachtwoord opnieuw in te stellen</p>
        </div>

        <div className="auth-card">
          <h2 id="forgot-password-title">Wachtwoord vergeten?</h2>

          <form className="auth-form" onSubmit={handleSubmit}>
            <label className="auth-field">
              <span>E-mailadres</span>
              <span className="auth-input-wrap">
                <Mail aria-hidden="true" />
                <input
                  type="email"
                  placeholder="naam@voorbeeld.nl"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  autoComplete="email"
                  required
                />
              </span>
            </label>

            {message && <p className="auth-success">{message}</p>}
            {devResetUrl && (
              <p className="auth-dev-note">
                Development resetlink: <Link to={devResetUrl}>{devResetUrl}</Link>
              </p>
            )}
            {error && <p className="auth-error">{error}</p>}

            <button className="auth-submit" type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Bezig..." : "Resetlink aanvragen"}
            </button>
          </form>

          <p className="auth-switch">
            Weet je je wachtwoord weer? <Link to="/login">Terug naar login</Link>
          </p>
        </div>
      </section>
    </main>
  )
}
