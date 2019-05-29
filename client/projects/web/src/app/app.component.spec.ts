import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { AppCommonModule } from './common/app-common.module';
import { AppComponent } from './app.component';

describe('AppComponent', () => {

    let fixture: ComponentFixture<AppComponent>;
    let target: AppComponent;
    let targetEl: HTMLElement;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            imports: [
                RouterTestingModule,
                AppCommonModule
            ],
            declarations: [
                AppComponent
            ],
        }).compileComponents();
        fixture = TestBed.createComponent(AppComponent);
        target = fixture.componentInstance;
        targetEl = fixture.debugElement.nativeElement;
    }));

    it('should create the app', async(() => {
        expect(target).toBeTruthy();
    }));

    it(`should has a navbar-brand 'Angular App Seed'`, async(() => {
        expect(targetEl.querySelector('a.navbar-brand').textContent)
            .toContain('Angular App Seed');
    }));

});
