import { TestBed } from '@angular/core/testing';

import { ApiInterceptor } from './api.interceptor';

describe('ApiInterceptor', () => {
    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: ApiInterceptor
            = TestBed.inject(ApiInterceptor);
        expect(service).toBeTruthy();
    });
});
