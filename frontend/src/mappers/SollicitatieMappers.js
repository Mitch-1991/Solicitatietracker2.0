export const emptyFormData = {
    bedrijf: "",
    functie: "",
    jobUrl: "",
    status: "",
    datum: "",
    priority: "",
    locatie: "",
    salarisMin: "",
    salarisMax: "",
    contactpersoon: "",
    contactEmail: "",
    volgendeStap: "",
    beschrijving: "",
};

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
export function mapFormDataToUpdateDto(data) {
    return {
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

export function mapCreatedApplicationToOverviewItem(application, bedrijf) {
    return {
        id: application.id,
        bedrijf: bedrijf.trim(),
        functie: application.jobTitle,
        status: application.status,
        datum: application.appliedDate,
        volgendeStap: application.nextStep,
    };
}

export function mapApplicationToFormData(application) {
    if (!application) {
        return emptyFormData;
    }

    return {
        bedrijf: application.bedrijf ?? "",
        functie: application.functie ?? application.jobTitle ?? "",
        jobUrl: application.jobUrl ?? "",
        status: application.status ?? "",
        datum: application.datum ?? application.appliedDate ?? "",
        priority: application.priority ?? "",
        locatie: application.locatie ?? application.location ?? "",
        salarisMin: application.salarisMin?.toString() ?? application.salaryMin?.toString() ?? "",
        salarisMax: application.salarisMax?.toString() ?? application.salaryMax?.toString() ?? "",
        contactpersoon: application.contactpersoon ?? application.contactPerson ?? "",
        contactEmail: application.contactEmail ?? "",
        volgendeStap: application.volgendeStap ?? application.nextStep ?? "",
        beschrijving: application.beschrijving ?? application.omschrijving ?? "",
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
