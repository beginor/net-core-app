import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LockComponent } from './lock.component';

describe('LockComponent', () => {
    let component: LockComponent;
    let fixture: ComponentFixture<LockComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [LockComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(LockComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
