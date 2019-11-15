import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';

import { AppSharedModule, ApiInterceptor } from 'app-shared';

import { MatModule } from './mat/mat.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

import { environment } from '../environments/environment';

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
            provide: 'apiRoot',
            useFactory: () => {
                let url = environment.apiRoot;
                if (url.startsWith('/')) {
                    url = `${location.protocol}//${location.host}${url}`;
                }
                return url;
            }
        }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
