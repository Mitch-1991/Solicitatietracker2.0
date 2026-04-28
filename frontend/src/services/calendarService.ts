import { apiFetch } from "./apiClient";
import { apiUrl } from "../config/api";

import type { CalendarInterviewResponse } from "../types/calendar";

const API_URL = apiUrl("/calendar");

export const getCalendarInterviews = async (from: string, to: string): Promise<CalendarInterviewResponse[]> => {
    const response: Response = await apiFetch(`${API_URL}/interviews?from=${from}&to=${to}`);

    if (!response.ok) {
        throw new Error("fout bij het ophalen van kalenderinterviews");
    }

    const data: CalendarInterviewResponse[] = await response.json();
    return data;
};
