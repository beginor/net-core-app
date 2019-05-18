import { TestBed } from '@angular/core/testing';

import { ApiInterceptorService } from './api-interceptor.service';

describe('ApiInterceptorService', () => {
    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: ApiInterceptorService
            = TestBed.get(ApiInterceptorService);
        expect(service).toBeTruthy();
    });
});
