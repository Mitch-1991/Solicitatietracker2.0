import {
  FileText,
  CheckSquare,
  Calendar,
  Clock,
} from "lucide-react";

import type {
  DashboardKpiResponse,
  DashboardOverviewItem,
  DashboardOverviewResponse,
  UpcomingInterviewResponse,
  UpcomingInterview,
  MappedKpi,
} from "../types/dashboard"; 


export const MapKPIs = (data: DashboardKpiResponse): MappedKpi[] => [
    {
      id: 1,
      label: "Lopende sollicitaties",
      value: data.lopendeSollicitaties,
      icon: FileText,
      color: "var(--kpi-blue)",
    },
    {
      id: 2,
      label: "Gesprekken gepland",
      value: data.gesprekkenGepland,
      icon: Calendar,
      color: "var(--kpi-green)",
    },
    {
      id: 3,
      label: "Afgewezen",
      value: data.afgewezen,
      icon: Clock,
      color: "var(--kpi-red)",
    },
    {
      id: 4,
      label: "Aanbiedingen",
      value: data.aanbiedingen,
      icon: CheckSquare,
      color: "var(--kpi-purple)",
    },
]

export const MapOverview = (data: DashboardOverviewResponse[] = []): DashboardOverviewItem[] =>
    data.map((item: DashboardOverviewResponse): DashboardOverviewItem => ({
        id: item.id,
        companyName: item.companyName,
        jobTitle: item.jobTitle,
        status: item.status,
        appliedDate: item.appliedDate,
        nextStep: item.nextStep,
    }))

export const MapUpcomingInterviews = (data: UpcomingInterviewResponse[] = []): UpcomingInterview[] =>
    data.map((item: UpcomingInterviewResponse): UpcomingInterview => ({
        id: item.id,
        companyName: item.companyName,
        jobTitle: item.jobTitle,
        interviewDate: item.interviewDate,
        hour: item.hour,
    }))
