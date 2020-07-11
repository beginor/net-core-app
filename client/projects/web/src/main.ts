import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

declare function isSupportedBrowser(): boolean;

if (isSupportedBrowser()) {
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
