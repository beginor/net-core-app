import { HomePage } from './home.po';

describe('gishub Home', () => {

    let page: HomePage;

    beforeEach(() => {
        page = new HomePage();
    });

    it('should display a button', () => {
        page.navigateTo();
        const text = page.getHelloButtonText();
        expect(text).toEqual('Hello, Angular !');
    });

    it('should change button text on click', () => {
        page.navigateTo();
        page.clickHelloButton();
        const text = page.getHelloButtonText();
        expect(text).toEqual('You have clicked 1 times!');
    });

});
