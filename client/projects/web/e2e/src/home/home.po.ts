import { browser, by, element, promise } from 'protractor';

export class HomePage {

    public navigateTo(): promise.Promise<void> {
        return browser.get('/home');
    }

    public getHelloButtonText(): promise.Promise<string> {
        return element(by.css('app-home button.btn')).getText();
    }

    public clickHelloButton(): void {
        element(by.css('app-home button.btn')).click();
    }

}
