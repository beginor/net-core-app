import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';

import { HomeComponent } from './home.component';

describe('HomeComponent', () => {

    let fixture: ComponentFixture<HomeComponent>;
    let target: HomeComponent;
    let targetEl: HTMLElement;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            imports: [
                NoopAnimationsModule,
                RouterTestingModule
            ],
            declarations: [
                HomeComponent
            ]
        }).compileComponents();
        // create test target;
        fixture = TestBed.createComponent(HomeComponent);
        target = fixture.componentInstance;
        targetEl = fixture.debugElement.nativeElement;
        fixture.detectChanges();
    }));

    it('should create HomeComponent', async(() => {
        expect(target).toBeTruthy();
    }));

    it(`should has a button`, async(() => {
        fixture.detectChanges();
        const btn = targetEl.querySelector('button.btn');
        expect(btn).toBeTruthy();
        expect(btn.textContent).toEqual('Hello, Angular !');
    }));

    it(`should change button content after click`, async(() => {
        fixture.detectChanges();
        const btn = targetEl.querySelector('button.btn');
        btn.dispatchEvent(new Event('click'));
        fixture.detectChanges();
        expect(btn.textContent).toContain('You have clicked');
    }));

});
