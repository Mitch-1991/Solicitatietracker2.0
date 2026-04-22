import Header from "./components/Header.tsx"
import Dashboard from "./pages/Dashboard.tsx"
import SideBar from "./components/SideBar.tsx"
import Application from "./pages/Application.tsx"
import { useState, useEffect } from "react"
import { MapOverview } from "./mappers/dashboardMappers.ts"
import { getDashboardOverview } from "./services/dashboardService.ts"
import {Routes, Route, Navigate} from "react-router-dom"

import type { DashboardOverviewItem, DashboardOverviewResponse } from "./types/dashboard.ts"




export default function App() {

  const [overview, setOverview] = useState<DashboardOverviewItem[]>([])

  useEffect(() => {
    const fetchOverview = async (): Promise<void> => {
      const data: DashboardOverviewResponse[] = await getDashboardOverview()
      const mappedOverview: DashboardOverviewItem[] = MapOverview(data)
      setOverview(mappedOverview)
    };
    fetchOverview()
  }, [])

  return (
    <>
      <Header />
      <section className="main-content">
        <SideBar />

        <Routes>
          <Route path="/" element={<Navigate to="/dashboard" replace/>} />
          <Route path="/dashboard" element={<Dashboard overview={overview} />} />
          <Route path="/applications" element={<Application overview={overview} setOverview={setOverview}/>} />
        </Routes>

      </section>
    </>

  )
}
