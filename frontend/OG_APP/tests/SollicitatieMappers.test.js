import test from "node:test";
import assert from "node:assert/strict";
import {
    mapCreatedApplicationToOverviewItem,
    mapFormDataToCreateDto,
} from "../src/mappers/SollicitatieMappers.js";

test("mapFormDataToCreateDto trims and converts form fields", () => {
    const result = mapFormDataToCreateDto({
        bedrijf: "  Acme  ",
        functie: "  QA Engineer ",
        jobUrl: "  https://example.com/job  ",
        status: "Gesprek",
        datum: "2026-03-31",
        priority: " hoog ",
        locatie: " Brussel ",
        salarisMin: "3500",
        salarisMax: "4200",
        contactpersoon: "",
        contactEmail: "",
        volgendeStap: "  Bel recruiter  ",
        beschrijving: "  Notitie  ",
    });

    assert.deepEqual(result, {
        userId: 1,
        bedrijf: "Acme",
        jobTitle: "QA Engineer",
        jobUrl: "https://example.com/job",
        location: "Brussel",
        status: 1,
        priority: "hoog",
        appliedDate: "2026-03-31",
        nextStep: "Bel recruiter",
        salaryMin: 3500,
        salaryMax: 4200,
        omschrijving: "Notitie",
    });
});

test("mapFormDataToCreateDto throws on unsupported status", () => {
    assert.throws(
        () =>
            mapFormDataToCreateDto({
                bedrijf: "Acme",
                functie: "QA Engineer",
                jobUrl: "",
                status: "Concept",
                datum: "2026-03-31",
                priority: "",
                locatie: "",
                salarisMin: "",
                salarisMax: "",
                contactpersoon: "",
                contactEmail: "",
                volgendeStap: "",
                beschrijving: "",
            }),
        /Ongeldige status/
    );
});

test("mapCreatedApplicationToOverviewItem keeps table shape aligned", () => {
    const result = mapCreatedApplicationToOverviewItem(
        {
            id: 12,
            jobTitle: "Platform Engineer",
            status: "Aanbieding",
            appliedDate: "2026-03-30",
            nextStep: "Contract bespreken",
        },
        "OpenAI"
    );

    assert.deepEqual(result, {
        id: 12,
        bedrijf: "OpenAI",
        functie: "Platform Engineer",
        status: "Aanbieding",
        datum: "2026-03-30",
        volgendeStap: "Contract bespreken",
    });
});
