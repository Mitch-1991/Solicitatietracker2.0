export type ApplicationStatus = 
| "verzonden"
| "gesprek"
| "afgewezen"
| "aanbieding";

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