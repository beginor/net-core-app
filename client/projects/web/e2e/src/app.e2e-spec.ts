import { AppPage } from './app.po';

describe('gishub App', () => {

    let page: AppPage;

    beforeEach(() => {
        page = new AppPage();
    });

    it('should display Navbar Brand', async () => {
        await page.navigateTo();
        expect(await page.getNavbarBrand()).toContain('Angular App Seed');
    });

});
