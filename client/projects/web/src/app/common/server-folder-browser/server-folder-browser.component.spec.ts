import { ComponentFixture, TestBed } from '@angular/core/testing';

import {
    ServerFolderBrowserComponent
} from './server-folder-browser.component';

describe('ServerFolderBrowserComponent', () => {
    let component: ServerFolderBrowserComponent;
    let fixture: ComponentFixture<ServerFolderBrowserComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [ServerFolderBrowserComponent]
        })
        .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(ServerFolderBrowserComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
