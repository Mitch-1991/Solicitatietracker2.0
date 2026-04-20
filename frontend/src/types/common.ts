export type ApplicationStatus = 
| "verzonden"
| "gesprek"
| "afgewezen"
| "aanbieding";

export type SelectOption = {
    value: string;
    label: string;
}

export type ApiError = {
    message: string;
    error: string;
}

export type ApiResponse<T> = {
    data: T;
    message?: string;
    success: boolean;
}