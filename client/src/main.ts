import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { registerLocaleData } from '@angular/common';
import zh from '@angular/common/locales/zh';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
    enableProdMode();
}

registerLocaleData(zh);

platformBrowserDynamic().bootstrapModule(AppModule)
    .then(() => {
        // console.log('app bootstrap');
    })
    .catch(err => {
        console.error(err);
    });
