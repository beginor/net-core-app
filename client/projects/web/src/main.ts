import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { registerLocaleData } from '@angular/common';
import zh from '@angular/common/locales/zh-Hans';
import zhExtra from '@angular/common/locales/extra/zh-Hans';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
    enableProdMode();
}

registerLocaleData(zh, 'zh-Hans', zhExtra);

platformBrowserDynamic().bootstrapModule(AppModule)
    .then(() => {
        // console.log('app bootstrap');
    })
    .catch(err => {
        console.error(err);
    });
