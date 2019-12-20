import { Component, ViewChild } from '@angular/core';

import { AccountService } from 'app-shared';
import { UiService } from './common/services/ui.service';
import { NavigationService } from './common/services/navigation.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {

    constructor(
        account: AccountService,
        public ui: UiService,
        public navigation: NavigationService
    ) {
        account.getInfo().catch(ex => {
            console.error('get account info with error: ', ex);
        });
    }

}
