import { CommonModule } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule, LOCALE_ID, ErrorHandler } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule, ApiInterceptor, HttpErrorHandler } from 'app-shared';

import { AppCommonModule } from './common';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { environment } from '../environments/environment';

@NgModule({
    declarations: [
        AppComponent,
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        CommonModule,
        FormsModule,
        HttpClientModule,
        NgbAlertModule,
        AppSharedModule,
        AppCommonModule,
        AppRoutingModule
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: ApiInterceptor,
            multi: true
        },
        {
            provide: LOCALE_ID,
            useValue: 'zh-Hans'
        },
        {
            provide: 'apiRoot',
            useFactory: () => '/net-core-app/api'
        },
        {
            provide: 'isProduction',
            useFactory: () => ['127.0.0.1', 'localhost'].indexOf(location.hostname) === -1 // eslint-disable-line max-len
        },
        {
            provide: ErrorHandler,
            useClass: HttpErrorHandler
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {}
