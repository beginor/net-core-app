import { TestBed } from '@angular/core/testing';

import { AuditLogsService } from './audit-logs.service';

describe('AuditLogsService', () => {
    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: AuditLogsService = TestBed.inject(AuditLogsService);
        expect(service).toBeTruthy();
    });
});
