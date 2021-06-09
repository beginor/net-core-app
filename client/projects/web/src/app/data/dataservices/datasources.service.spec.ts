import { TestBed } from '@angular/core/testing';

import { DataSourceService } from './datasources.service';

describe('DataSourceService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: DataSourceService = TestBed.inject(DataSourceService);
        expect(service).toBeTruthy();
    });

});
