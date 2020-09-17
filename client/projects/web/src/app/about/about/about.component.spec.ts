import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';

import { AboutComponent } from './about.component';

describe('AboutComponent', () => {

    let fixture: ComponentFixture<AboutComponent>;
    let component: AboutComponent;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [AboutComponent],
            imports: [
                NoopAnimationsModule,
                RouterTestingModule
            ]
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(AboutComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    // should display a header
    it('should display header with "Angular App Seed"', () => {
        const el = fixture.nativeElement;
        const h1 = el.querySelector('h1');
        expect(h1).toBeTruthy();
        expect(h1.textContent).toContain('Angular App Seed');
    });
});
