const API_URL = "http://localhost:5158/api/dashboard"

export const getDashboardKpis = async () => {
    const response = await fetch(`${API_URL}/kpis`);

    if(!response.ok){
        throw new Error("fout bij het ophalen van KPI's")
    }

    const data = await response.json()
    return data
}