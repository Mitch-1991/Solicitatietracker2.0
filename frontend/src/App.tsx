import Header from "./components/Header.tsx"
import Dashboard from "./pages/Dashboard.tsx"
import SideBar from "./components/SideBar.tsx"
import Application from "./pages/Application.tsx"
import Login from "./pages/Login.tsx"
import Register from "./pages/Register.tsx"
import ForgotPassword from "./pages/ForgotPassword.tsx"
import ResetPassword from "./pages/ResetPassword.tsx"
import Settings from "./pages/Settings.tsx"
import Calendar from "./pages/Calendar.tsx"
import ArchivedApplications from "./pages/ArchivedApplications.tsx"
import ProtectedRoute from "./components/ProtectedRoute.tsx"
import { useState, useEffect } from "react"
import { MapOverview } from "./mappers/dashboardMappers.ts"
import { getDashboardOverview } from "./services/dashboardService.ts"
import {Routes, Route, Navigate, useLocation} from "react-router-dom"
import { useAuth } from "./context/AuthContext.tsx"

import type { DashboardOverviewItem, DashboardOverviewResponse } from "./types/dashboard.ts"

export default function App() {
  const { isAuthenticated } = useAuth()
  const location = useLocation()
  const isAuthRoute = location.pathname === "/login" || location.pathname === "/register" || location.pathname === "/forgot-password" || location.pathname === "/reset-password"
  const [overview, setOverview] = useState<DashboardOverviewItem[]>([])

  useEffect(() => {
    if(!isAuthenticated){
      return
    }
    const fetchOverview = async (): Promise<void> => {
      const data: DashboardOverviewResponse[] = await getDashboardOverview()
      const mappedOverview: DashboardOverviewItem[] = MapOverview(data)
      setOverview(mappedOverview)
    };
    fetchOverview()
  }, [isAuthenticated])

  if (isAuthRoute){
    return (
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/reset-password" element={<ResetPassword />} />
      </Routes>
    )
  }

  return (
    <>
      <Header />
      <section className="main-content">
        <SideBar />

        <Routes>
          <Route path="/" element={<Navigate to="/dashboard" replace/>} />
          <Route path="/dashboard" element={
            <ProtectedRoute>
              <Dashboard overview={overview} />
            </ProtectedRoute>
          } />
          <Route path="/applications" element={
            <ProtectedRoute>
              <Application overview={overview} setOverview={setOverview}/>
            </ProtectedRoute>
          } />
          <Route path="/applications/archive" element={
            <ProtectedRoute>
              <ArchivedApplications />
            </ProtectedRoute>
          } />
          <Route path="/settings" element={
            <ProtectedRoute>
              <Settings />
            </ProtectedRoute>
          } />
          <Route path="/calendar" element={
            <ProtectedRoute>
              <Calendar />
            </ProtectedRoute>
          } />
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>

      </section>
    </>

  )
}
