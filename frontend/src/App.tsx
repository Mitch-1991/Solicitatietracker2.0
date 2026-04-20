import Header from "./Components/Header"
import Dashboard from "./pages/Dashboard"
import SideBar from "./Components/SideBar"
import Sollicitaties from "./pages/Sollicitaties"
import { useState, useEffect } from "react"
import { MapOverview } from "./mappers/dashboardMappers.js"
import { getDashboardOverview } from "./Services/dashboardService.js";




export default function App() {

  const [overview, setOverview] = useState([])

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
       <Sollicitaties overview={overview} setOverview={setOverview} />
        {/* <Dashboard overview={overview} />  */}
      </section>
    </>

  )
}
