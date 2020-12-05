import { TestBed } from '@angular/core/testing';

import { ConnectionStringService } from './connection-strings.service';

describe('ConnectionStringService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: ConnectionStringService = TestBed.get(ConnectionStringService);
        expect(service).toBeTruthy();
    });

});