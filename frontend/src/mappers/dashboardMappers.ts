import {
  FileText,
  CheckSquare,
  Calendar,
  Clock,
} from "lucide-react";

export const MapKPIs = (data) => [
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

export const MapOverview = (data = []) =>
    data.map((item) => ({
        id: item.id,
        bedrijf: item.bedrijf,
        functie: item.functie,
        status: item.status,
        datum: item.appliedDate,
        volgendeStap: item.nextStep,
    }))

export const MapUpcomingInterviews = (data = []) =>
    data.map((item) => ({
        id:2,
        bedrijf: item.bedrijf,
        functie: item.functie,
        datum: item.datum,
        uur: item.uur,
    }))