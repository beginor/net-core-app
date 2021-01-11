import { browser, by, element } from 'protractor';

export class AppPage {

    public async navigateTo(): Promise<unknown> {
        browser.sleep(1000);
        return browser.get(browser.baseUrl);
    }

    public async getNavbarBrand(): Promise<string> {
        return element(by.css('app-root a.navbar-brand')).getText();
    }

}
