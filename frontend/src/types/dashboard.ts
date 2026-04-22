import type { ApplicationStatus } from "./common";
import type { LucideIcon } from "lucide-react";

export interface DashboardOverviewItem{
    id: number;
    companyName: string;
    jobTitle: string;
    status: ApplicationStatus;
    appliedDate: string | null;
    nextStep: string | null;
}

export interface DashboardOverviewResponse {
    id: number;
    companyName: string;
    jobTitle: string;
    status: ApplicationStatus;
    appliedDate: string | null;
    nextStep: string | null;
}

export interface UpcomingInterview {
    id: number;
    companyName: string;
    jobTitle: string;
    interviewDate: string;
    hour: string;
}
export interface UpcomingInterviewResponse {
    id: number;
    companyName: string;
    jobTitle: string;
    interviewDate: string;
    hour: string;
}

export interface DashboardKpiResponse {
    lopendeSollicitaties: number;
    gesprekkenGepland: number;
    afgewezen: number;
    aanbiedingen: number;
}

export interface MappedKpi  {
  id: number;
  label: string;
  value: number;
  icon: LucideIcon;
  color: string;
}
