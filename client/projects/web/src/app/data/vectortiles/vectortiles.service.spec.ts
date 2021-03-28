import { TestBed } from '@angular/core/testing';

import { VectortileService } from './vectortiles.service';

describe('VectortileService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: VectortileService = TestBed.inject(VectortileService);
        expect(service).toBeTruthy();
    });

});