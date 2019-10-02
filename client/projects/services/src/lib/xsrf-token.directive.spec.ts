import { XsrfTokenDirective } from './xsrf-token.directive';

describe('XsrfTokenDirective', () => {
    it('should create an instance', () => {
        const directive = new XsrfTokenDirective(null);
        expect(directive).toBeTruthy();
    });
});
