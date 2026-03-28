export const MapOverview = (data = []) =>
    data.map((item) => ({
        id: item.id,
        bedrijf: item.bedrijf,
        functie: item.functie,
        status: item.status,
        datum: item.appliedDate,
        volgendeStap: item.nextStep,
    }))
