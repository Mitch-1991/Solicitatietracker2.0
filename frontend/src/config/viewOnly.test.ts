import { describe, it, expect, vi, beforeEach } from 'vitest';

vi.mock('../config/viewOnly', async (importOriginal) => {
    const actual = await importOriginal<typeof import('./viewOnly')>();
    return actual;
});

import { isViewOnlyEmail } from './viewOnly';

describe('isViewOnlyEmail', () => {
    beforeEach(() => {
        vi.stubEnv('VITE_VIEW_ONLY_EMAIL', 'recruiter.demo@sollicitatietracker.be');
    });

    it('returns true for the exact view-only email', () => {
        expect(isViewOnlyEmail('recruiter.demo@sollicitatietracker.be')).toBe(true);
    });

    it('is case-insensitive', () => {
        expect(isViewOnlyEmail('RECRUITER.DEMO@SOLLICITATIETRACKER.BE')).toBe(true);
        expect(isViewOnlyEmail('Recruiter.Demo@Sollicitatietracker.Be')).toBe(true);
    });

    it('ignores surrounding whitespace', () => {
        expect(isViewOnlyEmail('  recruiter.demo@sollicitatietracker.be  ')).toBe(true);
    });

    it('returns false for a different email', () => {
        expect(isViewOnlyEmail('user@example.com')).toBe(false);
    });

    it('returns false for undefined', () => {
        expect(isViewOnlyEmail(undefined)).toBe(false);
    });

    it('returns false for null', () => {
        expect(isViewOnlyEmail(null)).toBe(false);
    });

    it('returns false for an empty string', () => {
        expect(isViewOnlyEmail('')).toBe(false);
    });
});
