import { TestBed } from '@angular/core/testing';

import { XsrfService } from './xsrf.service';

describe('XsrfService', () => {
    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: XsrfService = TestBed.get(XsrfService);
        expect(service).toBeTruthy();
    });
});
