import { describe, it, expect } from 'vitest';
import { isViewOnlyEmail } from './viewOnly';

describe('isViewOnlyEmail', () => {
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

    it('works without VITE_VIEW_ONLY_EMAIL env var set (hardcoded fallback)', () => {
        expect(isViewOnlyEmail('recruiter.demo@sollicitatietracker.be')).toBe(true);
        expect(isViewOnlyEmail('other@example.com')).toBe(false);
    });
});
