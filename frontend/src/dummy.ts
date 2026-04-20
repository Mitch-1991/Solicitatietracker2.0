type DummySollicitatie = {
    id: number;
    bedrijf: string;
    functie: string;
    status: string;
    datum: string;
    uur: string;
}

export const Sollicitaties: DummySollicitatie[] = [
    {
        id:2,
        bedrijf: "TechCorp",
        functie: "Frontend Developer",
        status: "Verzonden",
        datum: "18 mrt 2026",
        uur: "14:00"
    },
    {
        id:2,
        bedrijf: "InnovativeHub",
        functie: "Junior React Developer",
        status: "Verzonden",
        datum: "18 mrt 2026",
        uur: "10:30"
    }
]
