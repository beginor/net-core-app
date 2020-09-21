import { TestBed } from '@angular/core/testing';

import { TileMapService } from './tile-maps.service';

describe('TileMapService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: TileMapService = TestBed.get(TileMapService);
        expect(service).toBeTruthy();
    });

});