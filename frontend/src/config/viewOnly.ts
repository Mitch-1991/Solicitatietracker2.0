const VIEW_ONLY_EMAIL = (import.meta.env.VITE_VIEW_ONLY_EMAIL as string | undefined)
    ?.toLowerCase()
    .trim() ?? '';

export function isViewOnlyEmail(email: string | undefined | null): boolean {
    if (!email || !VIEW_ONLY_EMAIL) return false;
    return email.toLowerCase().trim() === VIEW_ONLY_EMAIL;
}
