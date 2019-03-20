import { browser, by, element } from 'protractor';

export class AppPage {

    public navigateTo() {
        browser.sleep(1000);
        return browser.get('/');
    }

    public getNavbarBrand() {
        return element(by.css('app-root a.navbar-brand')).getText();
    }

}
