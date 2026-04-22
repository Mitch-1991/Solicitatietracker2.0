import Header from "./components/Header.tsx"
import Dashboard from "./pages/Dashboard.tsx"
import SideBar from "./components/SideBar.tsx"
// import Application from "./pages/Application.tsx"
import { useState, useEffect } from "react"
import { MapOverview } from "./mappers/dashboardMappers.ts"
import { getDashboardOverview } from "./services/dashboardService.ts"
import type { DashboardOverviewItem } from "./types/dashboard.ts"




export default function App() {

  const [overview, setOverview] = useState<DashboardOverviewItem[]>([])

  useEffect(() => {
    const fetchOverview = async (): Promise<void> => {
      const data = await getDashboardOverview()
      const mappedOverview = MapOverview(data)
      setOverview(mappedOverview)
    };
    fetchOverview()
  }, [])

  return (
    <>
      <Header />
      <section className="main-content">
        <SideBar />
       {/* <Application overview={overview} setOverview={setOverview} /> */}
        <Dashboard overview={overview} /> 
      </section>
    </>

  )
}
