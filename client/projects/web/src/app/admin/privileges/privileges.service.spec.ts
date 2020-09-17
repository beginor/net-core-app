import { TestBed } from '@angular/core/testing';

import { AppPrivilegeService } from './privileges.service';

describe('AppPrivilegeService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service = TestBed.inject(AppPrivilegeService);
        expect(service).toBeTruthy();
    });

});
