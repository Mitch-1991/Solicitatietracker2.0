import { apiFetch } from "./apiClient";
import { apiUrl } from "../config/api";

import type {companyDetailResponse} from "../types/company.ts";

const API_URL = apiUrl("/company")

export async function getCompanies(): Promise<companyDetailResponse[]> {
    const response = await apiFetch(`${API_URL}/companies`);
    if (!response.ok) {
        throw new Error("Failed to fetch companies");
    }
    const companies: companyDetailResponse[] = await response.json();
    return companies;
}
