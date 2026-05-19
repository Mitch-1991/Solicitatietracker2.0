import { test, expect } from '@playwright/test';

const VIEW_ONLY_EMAIL = 'recruiter.demo@sollicitatietracker.be';
const VIEW_ONLY_PASSWORD = process.env.VIEW_ONLY_PASSWORD ?? '';

async function loginAsViewOnly(page: import('@playwright/test').Page) {
    await page.goto('/#/login');
    await page.fill('input[type="email"]', VIEW_ONLY_EMAIL);
    await page.fill('input[type="password"]', VIEW_ONLY_PASSWORD);
    await page.click('button[type="submit"]');
    await page.waitForURL(/#\/dashboard/);
}

test.describe('View Only recruiter profile', () => {
    test.beforeEach(async ({ page }) => {
        await loginAsViewOnly(page);
    });

    test('shows "View Only" badge in the header', async ({ page }) => {
        await expect(page.locator('.header-view-only-badge')).toBeVisible();
        await expect(page.locator('.header-view-only-badge')).toContainText('View Only');
    });

    test('can browse the dashboard without errors', async ({ page }) => {
        await expect(page.locator('.dashboard-title')).toBeVisible();
    });

    test('can navigate to the applications page', async ({ page }) => {
        await page.goto('/#/applications');
        await expect(page.locator('.sollicitaties-page')).toBeVisible();
    });

    test('clicking "Nieuwe sollicitatie" shows the View Only modal, not the application modal', async ({ page }) => {
        await page.goto('/#/applications');

        const [request] = await Promise.all([
            page.waitForRequest(/\/api\/application/, { timeout: 2000 }).catch(() => null),
            page.click('.new-application-button'),
        ]);

        await expect(page.locator('[role="dialog"]')).toBeVisible();
        await expect(page.locator('#view-only-title')).toContainText('View Only profiel');
        expect(request).toBeNull();
    });

    test('View Only modal can be closed with the Sluiten button', async ({ page }) => {
        await page.goto('/#/applications');
        await page.click('.new-application-button');
        await expect(page.locator('[role="dialog"]')).toBeVisible();
        await page.click('button:has-text("Sluiten")');
        await expect(page.locator('[role="dialog"]')).not.toBeVisible();
    });

    test('View Only modal can be closed with the Escape key', async ({ page }) => {
        await page.goto('/#/applications');
        await page.click('.new-application-button');
        await expect(page.locator('[role="dialog"]')).toBeVisible();
        await page.keyboard.press('Escape');
        await expect(page.locator('[role="dialog"]')).not.toBeVisible();
    });

    test('View Only modal "Account aanmaken" link navigates to register page', async ({ page }) => {
        await page.goto('/#/applications');
        await page.click('.new-application-button');
        await page.click('a:has-text("Account aanmaken")');
        await expect(page).toHaveURL(/#\/register/);
    });

    test('can navigate to and browse the calendar', async ({ page }) => {
        await page.goto('/#/calendar');
        await expect(page.locator('main, .dashboard-container')).toBeVisible();
    });

    test('can navigate to settings and toggle dark mode', async ({ page }) => {
        await page.goto('/#/settings');
        const toggle = page.locator('.theme-toggle, [aria-label*="thema"], [aria-label*="dark"], [aria-label*="Donker"]').first();
        await expect(toggle).toBeVisible();
        await toggle.click();
    });

    test('submitting the password form shows the View Only modal and does not call the API', async ({ page }) => {
        await page.goto('/#/settings');

        const [request] = await Promise.all([
            page.waitForRequest(/\/api\/auth\/change-password/, { timeout: 2000 }).catch(() => null),
            (async () => {
                await page.fill('input[autocomplete="current-password"]', 'anything');
                await page.fill('input[autocomplete="new-password"]', 'newpassword');
                await page.fill('input[autocomplete="new-password"] + input, input[autocomplete="new-password"]:last-of-type', 'newpassword');
                await page.click('.settings-submit');
            })(),
        ]);

        await expect(page.locator('[role="dialog"]')).toBeVisible();
        await expect(page.locator('#view-only-title')).toContainText('View Only profiel');
        expect(request).toBeNull();
    });
});
