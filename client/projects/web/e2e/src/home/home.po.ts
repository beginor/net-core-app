import { browser, by, element } from 'protractor';

export class HomePage {

    public navigateTo(): Promise<unknown> {
        return browser.get('/home') as Promise<unknown>;
    }

    public getHelloButtonText(): Promise<string> {
        return element(by.css('app-home button.btn')).getText() as Promise<string>;
    }

    public clickHelloButton(): Promise<unknown> {
        return element(by.css('app-home button.btn')).click() as Promise<unknown>;
    }

}
