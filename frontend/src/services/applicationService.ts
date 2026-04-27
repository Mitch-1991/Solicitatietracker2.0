import { apiFetch } from "./apiClient";

import type {
    createdApplicationResponse,
    createApplicationDto,
    updateApplicationDto,
    ApplicationDetailResponse
} from "../types/application";

const API_URL = "http://localhost:5158/api/application"

function normalizeValidationMessage(field: string, message: string): string | null {
    const trimmedMessage = message.trim()

    if (!trimmedMessage) {
        return null
    }

    if (field === "createApplicationDto" && trimmedMessage.includes("field is required")) {
        return null
    }

    if (field === "$.status" && trimmedMessage.includes("could not be converted to Status")) {
        return "Status heeft een ongeldige waarde."
    }

    return trimmedMessage
}

function extractErrorMessage(errorData: unknown): string | null {
    if (!errorData || typeof errorData !== "object") {
        return null
    }

    if ("message" in errorData && typeof errorData.message === "string" && errorData.message.trim()) {
        return errorData.message
    }

    if ("errors" in errorData && errorData.errors && typeof errorData.errors === "object") {
        const messages = Object.entries(errorData.errors)
            .flatMap(([field, value]) => {
                if (!Array.isArray(value)) {
                    return []
                }

                return value
                    .filter((item): item is string => typeof item === "string")
                    .map((item) => normalizeValidationMessage(field, item))
                    .filter((item): item is string => Boolean(item))
            })

        const uniqueMessages = [...new Set(messages)]

        if (uniqueMessages.length > 0) {
            return uniqueMessages.join(" ")
        }
    }

    if ("title" in errorData && typeof errorData.title === "string" && errorData.title.trim()) {
        return errorData.title
    }

    return null
}

async function readErrorMessage(response: Response, fallbackMessage: string): Promise<string> {
    try {
        const contentType = response.headers.get("content-type") ?? ""

        if (contentType.includes("json")) {
            const errorData = await response.json()
            const parsedMessage = extractErrorMessage(errorData)
            if (parsedMessage) {
                return parsedMessage
            }
        }

        const errorText = await response.text()
        if (errorText.trim().startsWith("{")) {
            try {
                const errorData = JSON.parse(errorText)
                const parsedMessage = extractErrorMessage(errorData)

                if (parsedMessage) {
                    return parsedMessage
                }
            } catch {
                // Ignore invalid JSON in text responses.
            }
        }

        if (errorText.trim()) {
            return errorText
        }
    } catch {
        // Ignore parsing errors and fall back to the default message.
    }

    return fallbackMessage
}

export async function createApplication(applicationData: createApplicationDto): Promise<createdApplicationResponse> {
    const response: Response = await apiFetch (API_URL, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(applicationData)
    })
    if(!response.ok) {
        const errorMessage = await readErrorMessage(response, "Fout bij het aanmaken van de sollicitatie.")
        throw new Error(errorMessage)
    }
    return await response.json()
}
export async function updateApplication(id: number, applicationData: updateApplicationDto): Promise<createdApplicationResponse> {
    const response: Response = await apiFetch(`${API_URL}/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(applicationData)
    });

    if (!response.ok) {
        const errorMessage = await readErrorMessage(response, "Fout bij het bewerken van de sollicitatie.");
        throw new Error(errorMessage);
    }

    return await response.json();
}

export async function getApplicationById(id: number): Promise<ApplicationDetailResponse> {
    const response: Response = await apiFetch(`${API_URL}/${id}`);

    if (!response.ok) {
        const errorMessage = await readErrorMessage(response, "Fout bij het ophalen van de sollicitatie.");
        throw new Error(errorMessage);
    }

    const application: ApplicationDetailResponse = await response.json();
    return application;
}

export async function archiveApplication(id: number): Promise<void> {
    const response: Response = await apiFetch(`${API_URL}/${id}`, {
        method: "DELETE"
    });

    if (!response.ok) {
        const errorMessage = await readErrorMessage(response, "Fout bij het archiveren van de sollicitatie.");
        throw new Error(errorMessage);
    }
}

export async function getArchivedApplications(): Promise<ApplicationDetailResponse[]> {
    const response: Response = await apiFetch(`${API_URL}/archive`);

    if (!response.ok) {
        const errorMessage = await readErrorMessage(response, "Fout bij het ophalen van het archief.");
        throw new Error(errorMessage);
    }

    return await response.json();
}

export async function getArchivedApplicationById(id: number): Promise<ApplicationDetailResponse> {
    const response: Response = await apiFetch(`${API_URL}/archive/${id}`);

    if (!response.ok) {
        const errorMessage = await readErrorMessage(response, "Fout bij het ophalen van de gearchiveerde sollicitatie.");
        throw new Error(errorMessage);
    }

    return await response.json();
}
