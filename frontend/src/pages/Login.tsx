import { Briefcase, Lock, Mail } from "lucide-react"
import { useState } from "react"
import { useNavigate, Link } from "react-router-dom"
import { useAuth } from "../context/AuthContext"

export default function Login() {
  const { login } = useAuth()
  const navigate = useNavigate()

  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [rememberMe, setRememberMe] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault()
    setIsSubmitting(true)
    setError(null)

    try {
      await login({ email, password, rememberMe })
      navigate("/dashboard")
    } catch (err) {
      setError(err instanceof Error ? err.message : "Login mislukt.")
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-shell" aria-labelledby="login-title">
        <div className="auth-brand">
          <span className="auth-logo" aria-hidden="true">
            <Briefcase />
          </span>
          <h1>SollicitatieTracker</h1>
          <p>Beheer al je sollicitaties op één plek</p>
        </div>

        <div className="auth-card">
          <h2 id="login-title">Inloggen</h2>

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

            <label className="auth-field">
              <span>Wachtwoord</span>
              <span className="auth-input-wrap">
                <Lock aria-hidden="true" />
                <input
                  type="password"
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  autoComplete="current-password"
                  required
                />
              </span>
            </label>

            <div className="auth-row">
              <label className="auth-checkbox">
                <input
                  type="checkbox"
                  checked={rememberMe}
                  onChange={(e) => setRememberMe(e.target.checked)}
                />
                <span>Onthoud mij</span>
              </label>
              <Link to="/forgot-password" className="auth-link">Wachtwoord vergeten?</Link>
            </div>

            {error && <p className="auth-error">{error}</p>}

            <button className="auth-submit" type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Bezig..." : "Inloggen"}
            </button>
          </form>

          <p className="auth-switch">
            Nog geen account? <Link to="/register">Registreer hier</Link>
          </p>
        </div>
      </section>
    </main>
  )
}
