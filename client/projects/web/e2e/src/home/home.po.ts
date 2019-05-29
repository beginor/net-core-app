import { browser, by, element } from 'protractor';

export class HomePage {

    public navigateTo() {
        return browser.get('/home');
    }

    public getHelloButtonText() {
        return element(by.css('app-home button.btn')).getText();
    }

    public clickHelloButton() {
        element(by.css('app-home button.btn')).click();
    }

}
