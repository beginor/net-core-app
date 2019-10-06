import { TestBed } from '@angular/core/testing';

import { AntiforgeryService } from './xsrf.service';

describe('AntiforgeryService', () => {
    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: AntiforgeryService = TestBed.get(AntiforgeryService);
        expect(service).toBeTruthy();
    });
});
