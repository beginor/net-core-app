import { browser, by, element, promise } from 'protractor';

export class AppPage {
    public navigateTo(): Promise<void> {
        return browser.get(browser.baseUrl) as Promise<void>;
    }

    public getTitleText(): Promise<string> {
        return element(by.css('app-root h1')).getText() as Promise<string>;
    }
}
