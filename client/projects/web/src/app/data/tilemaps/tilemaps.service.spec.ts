import { TestBed } from '@angular/core/testing';

import { TileMapService } from './tilemaps.service';

describe('TileMapService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: TileMapService = TestBed.inject(TileMapService);
        expect(service).toBeTruthy();
    });

});
