import { TestBed } from '@angular/core/testing';

import { NavItemsService } from './nav-items.service';

describe('NavItemsService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: NavItemsService = TestBed.get(NavItemsService);
        expect(service).toBeTruthy();
    });

});
