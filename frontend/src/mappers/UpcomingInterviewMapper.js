export const MapUpcomingInterviews = (data = []) =>
    data.map((item) => ({
        id:2,
        bedrijf: item.bedrijf,
        functie: item.functie,
        datum: item.datum,
        uur: item.uur,
    }))