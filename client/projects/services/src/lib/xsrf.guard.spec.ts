import { TestBed } from '@angular/core/testing';

import { XsrfGuard } from './xsrf.service';

describe('XsrfGuard', () => {
    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: XsrfGuard = TestBed.get(XsrfGuard);
        expect(service).toBeTruthy();
    });
});
