import test from "node:test";
import assert from "node:assert/strict";
import { createApplication } from "../src/Services/SollicitatieService.js";

test("createApplication posts JSON and returns parsed response", async () => {
    const originalFetch = globalThis.fetch;
    const responseBody = { id: 7, jobTitle: "QA Engineer" };
    let fetchCall;

    globalThis.fetch = async (url, options) => {
        fetchCall = { url, options };

        return {
            ok: true,
            async json() {
                return responseBody;
            },
        };
    };

    try {
        const result = await createApplication({ bedrijf: "Acme" });

        assert.equal(
            fetchCall.url,
            "http://localhost:5158/api/application"
        );
        assert.equal(fetchCall.options.method, "POST");
        assert.equal(fetchCall.options.headers["Content-Type"], "application/json");
        assert.equal(fetchCall.options.body, JSON.stringify({ bedrijf: "Acme" }));
        assert.deepEqual(result, responseBody);
    } finally {
        globalThis.fetch = originalFetch;
    }
});

test("createApplication prefers backend message when request fails", async () => {
    const originalFetch = globalThis.fetch;

    globalThis.fetch = async () => ({
        ok: false,
        async json() {
            return { message: "Validatie mislukt." };
        },
    });

    try {
        await assert.rejects(
            () => createApplication({ bedrijf: "Acme" }),
            /Validatie mislukt\./
        );
    } finally {
        globalThis.fetch = originalFetch;
    }
});

test("createApplication falls back to default message when error JSON is invalid", async () => {
    const originalFetch = globalThis.fetch;

    globalThis.fetch = async () => ({
        ok: false,
        async json() {
            throw new Error("invalid json");
        },
    });

    try {
        await assert.rejects(
            () => createApplication({ bedrijf: "Acme" }),
            /Fout bij het aanmaken van de sollicitatie\./
        );
    } finally {
        globalThis.fetch = originalFetch;
    }
});
