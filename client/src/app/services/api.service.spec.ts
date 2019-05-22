import { TestBed } from '@angular/core/testing';

import { AuthInterceptor } from './api.interceptor';

describe('AuthInterceptor', () => {
    beforeEach(() => TestBed.configureTestingModule({}));

    it('should be created', () => {
        const service: AuthInterceptor = TestBed.get(AuthInterceptor);
        expect(service).toBeTruthy();
    });

});
