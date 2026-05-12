# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Solicitatietracker is a full-stack job application tracking tool. The frontend is a React/TypeScript SPA deployed to GitHub Pages; the backend is an ASP.NET Core 9.0 Web API deployed to MonsterASP with a SQL Server database.

## Commands

### Frontend (`/frontend`)

```bash
npm run dev       # Start Vite dev server on port 5173
npm run build     # TypeScript compile + Vite production build
npm run lint      # Run ESLint
npm run preview   # Preview production build locally
```

### Backend (`/backend`)

```bash
dotnet build Solicitatietracker2.0.sln          # Build entire solution
dotnet run --project Solicitatietracker_API     # Run the API (Swagger at /swagger)
dotnet test SolicitatieTracker.Tests            # Run xUnit tests
dotnet test SolicitatieTracker.Tests --filter "FullyQualifiedName~ClassName"  # Run a single test class
```

## Architecture

### Frontend

**Stack:** React 19, TypeScript, Vite, React Router DOM (HashRouter — required for GitHub Pages).

Key folders under `frontend/src/`:
- `pages/` — one file per route (Dashboard, ApplicationDetail, Login, Register, etc.)
- `components/` — shared UI pieces (Header, Sidebar, Modal, Table variants)
- `services/` — typed API call modules (`authService`, `applicationService`, `calendarService`, `companyService`, `dashboardService`)
- `context/AuthContext.tsx` — global auth state (JWT token + user info)
- `types/` — TypeScript interfaces mirroring backend DTOs
- `mappers/` — transform API response shapes to frontend models
- `config/` — `apiClient` wrapper with base URL and auth headers

The API base URL is set in `frontend/.env` (`VITE_API_BASE_URL`). Production points to `https://sollicitatietracker.runasp.net/api`.

### Backend

**Stack:** ASP.NET Core 9.0, Entity Framework Core 9.0, SQL Server, JWT Bearer auth, xUnit.

The solution follows Clean Architecture with four projects:

| Project | Responsibility |
|---|---|
| `Solicitatietracker_API` | Controllers, Program.cs (DI, CORS, Swagger, JWT), startup |
| `SolicitatieTracker.Application` | Business logic services, DTOs |
| `SolicitatieTracker.Domain` | Entities, Enums (no dependencies) |
| `SolicitatieTracker.Infrastructure` | DbContext, repositories, email outbox/SMTP |

**Email:** Uses the outbox pattern — emails are saved to the database first, then a background `IHostedService` processes and sends them via SMTP (MailKit). This decouples sending from request handling.

**Auth:** JWT tokens with two lifetimes — 60 minutes (default) and 30 days (Remember Me). Password reset uses email-based tokens. BCrypt is used for password hashing.

**Database:** LocalDB in development, SQL Server in production. `DatabaseInitializer` seeds initial data on startup.

### Deployment

- **Frontend:** Automatically deployed to GitHub Pages on push to `main` via `.github/workflows/deploy-frontend.yml`.
- **Backend:** Manually deployed to MonsterASP.

### Best-Practices
- Use comments sparingly. Only comment complex code
