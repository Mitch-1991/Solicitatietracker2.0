# Plan: Auth Uitbreidingen Voor SollicitatieTracker

## Summary
Werk drie auth-functies uit in een compacte iteratie: "Onthoud mij", "Wachtwoord vergeten?" en een minimale "Instellingen"-pagina met wachtwoord wijzigen.

## Key Changes
- "Onthoud mij": `LoginRequest` bevat `rememberMe`, de frontend bewaart korte sessies in `sessionStorage` en langere sessies in `localStorage`, en de backend kiest de JWT-vervaldatum op basis van deze keuze.
- "Wachtwoord vergeten?": de backend maakt een tijdelijk reset-token aan, bewaart alleen de hash en retourneert in development een resetlink/token. De frontend bevat aanvraag- en resetpagina's.
- Instellingen: `/settings` is een beschermde pagina met accountgegevens en een formulier om het wachtwoord te wijzigen.

## Public Interfaces
- Frontend auth types/services bevatten requesttypes en methods voor `forgotPassword`, `resetPassword` en `changePassword`.
- Backend auth bevat endpoints voor `POST /api/auth/forgot-password`, `POST /api/auth/reset-password` en `POST /api/auth/change-password`.
- `IJwtTokenService` gebruikt `GenerateToken(User user, bool rememberMe)` en `GetExpiration(bool rememberMe)`.
- `User` bevat reset-tokenvelden: `PasswordResetTokenHash` en `PasswordResetTokenExpiresAt`.

## Test Plan
- Backend unit tests dekken remember-me expiratie, onbekende e-mail bij forgot password, geldig/verlopen reset-token en wachtwoord wijzigen.
- Frontend build valideert TypeScript en route-integratie.
- Backend testcommand: `dotnet test backend\Solicitatietracker2.0\SolicitatieTracker.Tests\SolicitatieTracker.Tests.csproj`.

## Assumptions
- Eerste versie gebruikt een development resetlink in de API-response, geen e-mailprovider.
- Wachtwoordbeleid blijft minimaal 6 tekens.
- Instellingen blijft minimaal: accountinfo en wachtwoord wijzigen.
- De bestaande SQL Server/localdb database krijgt ontbrekende resetkolommen bij startup via de seeder.
