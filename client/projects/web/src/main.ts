import { registerLocaleData } from '@angular/common';
import zh from '@angular/common/locales/zh-Hans';
import zhEx from '@angular/common/locales/extra/zh-Hans';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';

declare function isSupportedBrowser(): boolean;

if (isSupportedBrowser()) {

    registerLocaleData(zh, 'zh-Hans', zhEx);

    platformBrowserDynamic().bootstrapModule(AppModule)
        .then(() => {
            // console.log('app bootstrap');
        })
        .catch(err => {
            console.error(err);
        });
}
