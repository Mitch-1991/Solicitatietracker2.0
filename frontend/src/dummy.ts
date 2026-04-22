type DummySollicitatie = {
    id: number;
    bedrijf: string;
    functie: string;
    status: string;
    datum: string;
    uur: string;
}

export type DummyCompany = {
    id: number;
    companyName: string;
    websiteURL: string;
    location: string;
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

export const Companies: DummyCompany[] = [
    {
        id: 1,
        companyName: "TechCorp",
        websiteURL: "https://www.techcorp.com",
        location: "Antwerpen"
    },
    {
        id: 2,
        companyName: "InnovativeHub",
        websiteURL: "https://www.innovativehub.com",
        location: "Brussel"
    }
]
