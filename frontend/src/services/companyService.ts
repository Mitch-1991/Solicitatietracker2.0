import { apiFetch } from "./apiClient";

import type {companyDetailResponse} from "../types/company.ts";

const API_URL = "http://localhost:5158/api/company"

export async function getCompanies(): Promise<companyDetailResponse[]> {
    const response = await apiFetch(`${API_URL}/companies`);
    if (!response.ok) {
        throw new Error("Failed to fetch companies");
    }
    const companies: companyDetailResponse[] = await response.json();
    return companies;
}