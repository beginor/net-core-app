import { TestBed } from '@angular/core/testing';

import { RolesService } from './roles.service';

describe('RolesService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: RolesService = TestBed.get(RolesService);
        expect(service).toBeTruthy();
    });

});
