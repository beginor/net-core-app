import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavSidebarComponent } from './nav-sidebar.component';

describe('NavSidebarComponent', () => {
    let component: NavSidebarComponent;
    let fixture: ComponentFixture<NavSidebarComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [NavSidebarComponent]
        })
        .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(NavSidebarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
