import type { ApplicationStatus, priorityStatus } from "./common";

export interface OverviewApplication {
    id: number;
    company: string;
    function: string;
    status: ApplicationStatus;
    date: string;
    nextStep: string;    
}

export interface ApplicationFormData {
    companyName: string;
    jobTitle: string;
    jobUrl: string;
    status: ApplicationStatus;
    date: string;
    priority: priorityStatus;
    location: string;
    salarisMin: number;
    salarisMax: number;
    notes: string;
    contactPerson: string;
    contactEmail: string;
    nextStep: string;
}

export interface createApplicationDto {
    userId: number;
    companyName: string;
    jobTitle: string;
    jobUrl: string;
    status: ApplicationStatus;
    appliedDate: string;
    priority: priorityStatus;
    location: string;
    salarisMin: number;
    salarisMax: number;
    notes: string;
    nextStep: string;
}

export interface updateApplicationDto {
    companyName: string;
    jobTitle: string;
    jobUrl: string;
    status: ApplicationStatus;
    appliedDate: string;
    priority: priorityStatus;
    location: string;
    salarisMin: number;
    salarisMax: number;
    notes: string;
    nextStep: string;
}
