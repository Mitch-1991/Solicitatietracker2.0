import type { ApiApplicationStatus, ApplicationStatus, PriorityStatus } from "./common";

export type InterviewType = "Online" | "Op locatie";

export interface InterviewDto {
    interviewType: InterviewType;
    scheduledStart: string;
    scheduledEnd: string | null;
    location: string | null;
    meetingLink: string | null;
    contactPerson: string | null;
    contactEmail: string | null;
    notes: string | null;
}

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
    websiteURL: string | null;
    interviewType: InterviewType | "";
    interviewDate: string;
    interviewStartTime: string;
    interviewEndTime: string;
    interviewLocation: string;
    meetingLink: string;
    interviewContactPerson: string;
    interviewContactEmail: string;
    interviewNotes: string;
}

export interface createApplicationDto {
    userId: number;
    companyName: string;
    jobTitle: string;
    jobUrl: string | null;
    status: ApiApplicationStatus;
    appliedDate: string | null;
    priority: PriorityStatus | null;
    location: string | null;
    salaryMin: number | null;
    salaryMax: number | null;
    notes: string | null;
    nextStep: string | null;
    interview: InterviewDto | null;
}

export interface createdApplicationResponse {
    id: number;
    companyName: string;
    jobTitle: string;
    jobUrl: string | null;
    status: ApplicationStatus;
    location: string | null;
    appliedDate: string | null;
    priority: PriorityStatus | null;
    salaryMin: number | null;
    salaryMax: number | null;
    contactPerson: string | null;
    contactEmail: string | null;
    notes: string | null;
    nextStep: string | null;
    interview: InterviewDto | null;
    isArchived: boolean;
    archivedAt: string | null;
}

export interface updateApplicationDto {
    companyName: string;
    jobTitle: string;
    jobUrl: string | null;
    status: ApiApplicationStatus;
    appliedDate: string | null;
    priority: PriorityStatus | null;
    location: string | null;
    salaryMin: number | null;
    salaryMax: number | null;
    notes: string | null;
    nextStep: string | null;
    interview: InterviewDto | null;
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
    interview: InterviewDto | null;
    contactPerson: string | null;
    contactEmail: string | null;
    nextStep: string | null;
    isArchived: boolean;
    archivedAt: string | null;
}

