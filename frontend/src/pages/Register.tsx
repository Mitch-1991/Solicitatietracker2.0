import { Briefcase, Lock, Mail, User } from "lucide-react"
import React, { useState } from "react"
import { useNavigate, Link } from "react-router-dom"
import { useAuth } from "../context/authContextValue"

export default function Register() {
  const { register } = useAuth()
  const navigate = useNavigate()

  const [firstName, setFirstName] = useState("")
  const [lastName, setLastName] = useState("")
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const [error, setError] = useState("")
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault()
    setError("")

    if (password !== confirmPassword) {
      setError("Wachtwoorden komen niet overeen.")
      return
    }

    setIsSubmitting(true)

    try {
      await register({ firstName, lastName, email, password })
      navigate("/dashboard")
    } catch (err) {
      setError(err instanceof Error ? err.message : "Registratie mislukt.")
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-shell auth-shell-register" aria-labelledby="register-title">
        <div className="auth-brand">
          <span className="auth-logo" aria-hidden="true">
            <Briefcase />
          </span>
          <h1>SollicitatieTracker</h1>
          <p>Beheer al je sollicitaties op één plek</p>
        </div>

        <div className="auth-card">
          <h2 id="register-title">Registreren</h2>

          <form className="auth-form" onSubmit={handleSubmit}>
            <div className="auth-field-grid">
              <label className="auth-field">
                <span>Voornaam</span>
                <span className="auth-input-wrap">
                  <User aria-hidden="true" />
                  <input
                    type="text"
                    placeholder="Voornaam"
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                    autoComplete="given-name"
                    required
                  />
                </span>
              </label>

              <label className="auth-field">
                <span>Achternaam</span>
                <span className="auth-input-wrap">
                  <User aria-hidden="true" />
                  <input
                    type="text"
                    placeholder="Achternaam"
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                    autoComplete="family-name"
                    required
                  />
                </span>
              </label>
            </div>

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
                  autoComplete="new-password"
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
                  placeholder="••••••••"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  autoComplete="new-password"
                  required
                />
              </span>
            </label>

            {error && <p className="auth-error">{error}</p>}

            <button className="auth-submit" type="submit" disabled={isSubmitting}>
              {isSubmitting ? "Bezig..." : "Registreren"}
            </button>
          </form>

          <p className="auth-switch">
            Heb je al een account? <Link to="/login">Login hier</Link>
          </p>
        </div>
      </section>
    </main>
  )
}

