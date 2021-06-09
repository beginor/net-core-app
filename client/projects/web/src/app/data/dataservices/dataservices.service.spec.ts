import { TestBed } from '@angular/core/testing';

import { DataServiceService } from './dataservices.service';

describe('DataServiceService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: DataServiceService = TestBed.inject(DataServiceService);
        expect(service).toBeTruthy();
    });

});
