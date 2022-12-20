import { CommonModule } from '@angular/common';
import { NgModule, ErrorHandler, LOCALE_ID } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';

import {
    AppSharedModule, ApiInterceptor, HttpErrorHandler, isProd
} from 'app-shared';

import { MatModule } from './mat/mat.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent
    ],
    imports: [
        BrowserAnimationsModule,
        BrowserModule,
        CommonModule,
        FormsModule,
        HttpClientModule,
        AppSharedModule,
        AppRoutingModule,
        MatModule
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
            useValue: '/net-core-app/api'
        },
        {
            provide: 'isProduction',
            useFactory: isProd
        },
        {
            provide: ErrorHandler,
            useClass: HttpErrorHandler
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
