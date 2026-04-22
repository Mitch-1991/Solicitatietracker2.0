export type ApplicationStatus =
| "Verzonden"
| "Gesprek"
| "Afgewezen"
| "Aanbieding";

export type ApiApplicationStatus = 0 | 1 | 2 | 3;

export const applicationStatusToApiStatusMap: Record<ApplicationStatus, ApiApplicationStatus> = {
    Verzonden: 0,
    Gesprek: 1,
    Afgewezen: 2,
    Aanbieding: 3,
};

export type PriorityStatus = "hoog" | "gemiddeld" | "laag";

export interface SelectOption {
    value: string;
    label: string;
}

export interface ApiError {
    message: string;
    error: string;
}

export interface ApiResponse<T> {
    data: T;
    message?: string;
    success: boolean;
}
