import { enableProdMode } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import zh from '@angular/common/locales/zh-Hans';
import zhEx from '@angular/common/locales/extra/zh-Hans';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

declare function isSupportedBrowser(): boolean;

if (isSupportedBrowser()) {

    registerLocaleData(zh, 'zh-Hans', zhEx);

    if (environment.production) {
        enableProdMode();
    }

    platformBrowserDynamic().bootstrapModule(AppModule)
        .then(() => {
            // console.log('app bootstrap');
        })
        .catch(err => {
            console.error(err);
        });
}
