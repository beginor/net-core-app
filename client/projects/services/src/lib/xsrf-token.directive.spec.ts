import { AntiforgeryDirective } from './antiforgery.directive';

describe('AntiforgeryDirective', () => {
    it('should create an instance', () => {
        const directive = new AntiforgeryDirective(null);
        expect(directive).toBeTruthy();
    });
});
