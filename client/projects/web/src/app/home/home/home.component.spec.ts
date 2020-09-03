import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';

import { HomeComponent } from './home.component';

describe('HomeComponent', () => {

    let fixture: ComponentFixture<HomeComponent>;
    let target: HomeComponent;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                NoopAnimationsModule,
                RouterTestingModule
            ],
            declarations: [
                HomeComponent
            ]
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(HomeComponent);
        target = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create HomeComponent', () => {
        expect(target).toBeTruthy();
    });

    it(`should has a button`, () => {
        const targetEl = fixture.nativeElement;
        const btn = targetEl.querySelector('button.btn');
        expect(btn).toBeTruthy();
        expect(btn.textContent).toEqual('Hello, Angular !');
    });

    it(`should change button content after click`, () => {
        const targetEl = fixture.nativeElement;
        const btn = targetEl.querySelector('button.btn');
        btn.dispatchEvent(new Event('click'));
        fixture.detectChanges();
        expect(btn.textContent).toContain('You have clicked');
    });

});
