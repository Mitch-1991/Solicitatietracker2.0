export function mapFormDataToCreateDto(data) {
    return {
        userId: 1,
        bedrijf: data.bedrijf.trim(),
        jobTitle: data.functie.trim(),
        jobUrl: data.jobUrl.trim() || null,
        location: data.locatie.trim() || null,
        status: mapStatusToEnumValue(data.status),
        priority: data.priority.trim() || null,
        appliedDate: data.datum,
        nextStep: data.volgendeStap.trim() || null,
        salaryMin: data.salarisMin ? Number(data.salarisMin) : null,
        salaryMax: data.salarisMax ? Number(data.salarisMax) : null,
        omschrijving: data.beschrijving.trim() || null,
    };
}

function mapStatusToEnumValue(status) {
    const normalizedStatus = status.trim().toLowerCase();

    switch (normalizedStatus) {
        case "verzonden":
            return 0;
        case "gesprek":
            return 1;
        case "afgewezen":
            return 2;
        case "aanbieding":
            return 3;
        default:
            throw new Error("Ongeldige status. Gebruik: Verzonden, Gesprek, Afgewezen of Aanbieding.");
    }
}