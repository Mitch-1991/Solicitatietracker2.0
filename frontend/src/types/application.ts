import type { ApplicationStatus, PriorityStatus } from "./common";

export interface ApplicationFormData {
    companyName: string;
    jobTitle: string;
    jobUrl: string | null;
    status: ApplicationStatus | "";
    date: string | null;
    priority: PriorityStatus | "";
    location: string | null;
    salaryMin: number | "";
    salaryMax: number | "";
    notes: string;
    contactPerson: string;
    contactEmail: string;
    nextStep: string;
}

export interface createApplicationDto {
    userId: number;
    companyName: string;
    jobTitle: string;
    jobUrl: string | null;
    status: ApplicationStatus;
    appliedDate: string | null;
    priority: PriorityStatus | null;
    location: string | null;
    salaryMin: number | null;
    salaryMax: number | null;
    notes: string | null;
    nextStep: string | null;
}

export interface createdApplicationResponse {
    id: number;
    companyName: string;
    jobTitle: string;
    status: ApplicationStatus;
    appliedDate: string | null;
    priority: PriorityStatus | null;
    salaryMin: number | null;
    salaryMax: number | null;
    notes: string | null;
    nextStep: string | null;
}

export interface updateApplicationDto {
    companyName: string;
    jobTitle: string;
    jobUrl: string | null;
    status: ApplicationStatus;
    appliedDate: string | null;
    priority: PriorityStatus | null;
    location: string | null;
    salaryMin: number | null;
    salaryMax: number | null;
    notes: string | null;
    nextStep: string | null;
}

export interface ApplicationDetailResponse {
    id: number;
    companyName: string;
    jobTitle: string;
    jobUrl: string | null;
    status: ApplicationStatus;
    appliedDate: string | null;
    priority: PriorityStatus | null;
    location: string | null;
    salaryMin: number | null;
    salaryMax: number | null;
    notes: string | null;
    contactPerson: string | null;
    contactEmail: string | null;
    nextStep: string | null;
}
