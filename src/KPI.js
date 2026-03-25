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
      color: "#2B7FFF",
    },
    {
      id: 2,
      label: "Gesprekken gepland",
      value: "3",
      icon: Calendar,
      color: "#00C950",
    },
    {
      id: 3,
      label: "Afgewezen",
      value: "5",
      icon: Clock,
      color: "#FB2C36",
    },
    {
      id: 4,
      label: "Aanbiedingen",
      value: "2",
      icon: CheckSquare,
      color: "#AD46FF",
    },
]