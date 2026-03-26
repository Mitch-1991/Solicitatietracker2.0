import Header from "./Components/Header"
import Dashboard from "./Components/Dashboard"
import SideBar from "./Components/SideBar"



export default function App() {
  return (
    <>
      <Header />
      <section className="main-content">
        <SideBar />
        <Dashboard />
      </section>
    </>

  )
}