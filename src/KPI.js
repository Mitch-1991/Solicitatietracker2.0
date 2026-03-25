import {
  FileText,
  CheckSquare,
  Calendar,
  Clock,
} from "lucide-react";

export const KPIs = [
    {
      id: 1,
      label: "Lopende sollicitaties",
      value: "12",
      icon: FileText,
      color: "bg-blue-500",
    },
    {
      id: 2,
      label: "Gesprekken gepland",
      value: "3",
      icon: Calendar,
      color: "bg-green-500",
    },
    {
      id: 3,
      label: "Afgewezen",
      value: "5",
      icon: Clock,
      color: "bg-red-500",
    },
    {
      id: 4,
      label: "Aanbiedingen",
      value: "2",
      icon: CheckSquare,
      color: "bg-purple-500",
    },
]