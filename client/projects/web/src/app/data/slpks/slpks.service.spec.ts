import { TestBed } from '@angular/core/testing';

import { SlpkService } from './slpks.service';

describe('SlpkService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: SlpkService = TestBed.get(SlpkService);
        expect(service).toBeTruthy();
    });

});