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
      color: "#2B7FFF",
    },
    {
      id: 2,
      label: "Gesprekken gepland",
      value: data.gesprekkenGepland,
      icon: Calendar,
      color: "#00C950",
    },
    {
      id: 3,
      label: "Afgewezen",
      value: data.afgewezen,
      icon: Clock,
      color: "#FB2C36",
    },
    {
      id: 4,
      label: "Aanbiedingen",
      value: data.aanbiedingen,
      icon: CheckSquare,
      color: "#AD46FF",
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
        id:2,
        companyName: item.companyName,
        jobTitle: item.jobTitle,
        interviewDate: item.interviewDate,
        hour: item.hour,
    }))