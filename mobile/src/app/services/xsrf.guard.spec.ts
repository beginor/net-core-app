import { TestBed, async, inject } from '@angular/core/testing';

import { XsrfGuard } from './xsrf.guard';

describe('XsrfGuard', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [XsrfGuard]
        });
    });

    it('should ...', inject([XsrfGuard], (guard: XsrfGuard) => {
        expect(guard).toBeTruthy();
    }));
});
