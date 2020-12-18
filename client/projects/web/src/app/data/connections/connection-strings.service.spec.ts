import { TestBed } from '@angular/core/testing';

import { ConnectionService } from './connection-strings.service';

describe('ConnectionStringService', () => {

    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: ConnectionService = TestBed.inject(ConnectionService);
        expect(service).toBeTruthy();
    });

});
