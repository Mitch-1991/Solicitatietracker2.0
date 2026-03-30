const API_URL = "http://localhost:5158/api/dashboard"

export const getDashboardKpis = async () => {
    const response = await fetch(`${API_URL}/kpis`);

    if (!response.ok) {
        throw new Error("fout bij het ophalen van KPI's")
    }

    const data = await response.json()
    return data
}
export const getDashboardOverview = async () => {
    const response = await fetch(`${API_URL}/overview`);

    if (!response.ok) {
        throw new Error("fout bij het ophalen van overzicht")
    }
    const data = await response.json()
    return data
}
export const getUpcomingInterviews = async () => {
    const response = await fetch(`${API_URL}/upcoming-interviews`);

    if (!response.ok) {
        throw new Error("fout bij het ophalen van aankomende interviews")
    }
    const data = await response.json()
    return data

}