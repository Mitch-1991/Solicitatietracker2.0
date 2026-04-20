import type { ApplicationStatus } from "./common";

export interface DashboardOverviewItem{
    id: number;
    companyName: string;
    jobTitle: string;
    status: ApplicationStatus;
    appliedDate: string | null;
    nextStep: string;
}

export interface UpcomingInterview {
    id: number;
    companyName: string;
    jobTitle: string;
    interviewDate: string;
    hour: string;
}