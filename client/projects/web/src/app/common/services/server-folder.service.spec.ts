import { TestBed } from '@angular/core/testing';

import { ServerFolderService } from './server-folder.service';

describe('ServerFolderService', () => {
    let service: ServerFolderService;

    beforeEach(() => {
        TestBed.configureTestingModule({});
        service = TestBed.inject(ServerFolderService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });
});
