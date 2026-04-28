import { apiFetch } from "./apiClient";
import { apiUrl } from "../config/api";

import type {
    DashboardKpiResponse,
    DashboardOverviewResponse,
    UpcomingInterviewResponse
} from "../types/dashboard";

const API_URL = apiUrl("/dashboard")

export const getDashboardKpis = async (): Promise<DashboardKpiResponse> => {
    const response: Response = await apiFetch(`${API_URL}/kpis`);

    if (!response.ok) {
        throw new Error("fout bij het ophalen van KPI's")
    }

    const data: DashboardKpiResponse = await response.json()
    return data
}
export const getDashboardOverview = async (): Promise<DashboardOverviewResponse[]> => {
    const response: Response = await apiFetch(`${API_URL}/overview`);

    if (!response.ok) {
        throw new Error("fout bij het ophalen van overzicht")
    }
    const data: DashboardOverviewResponse[] = await response.json()
    return data
}
export const getUpcomingInterviews = async (): Promise<UpcomingInterviewResponse[]> => {
    const response: Response = await apiFetch(`${API_URL}/upcoming-interviews`);

    if (!response.ok) {
        throw new Error("fout bij het ophalen van aankomende interviews")
    }
    const data: UpcomingInterviewResponse[] = await response.json()
    return data

}
