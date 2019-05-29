import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';

import { AboutComponent } from './about.component';

describe('AboutComponent', () => {

    let target: AboutComponent;
    let fixture: ComponentFixture<AboutComponent>;
    let targetEl: HTMLElement;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [AboutComponent],
            imports: [
                NoopAnimationsModule,
                RouterTestingModule
            ]
        }).compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(AboutComponent);
        target = fixture.componentInstance;
        targetEl = fixture.debugElement.nativeElement;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(target).toBeTruthy();
    });

    // should display a header
    it('should display header with "Angular App Seed"', () => {
        const h1 = targetEl.querySelector('h1');
        expect(h1).toBeTruthy();
        expect(h1.textContent).toContain('Angular App Seed');
    });
});
