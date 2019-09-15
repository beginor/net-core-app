import { Component } from '@angular/core';

import { AccountService } from 'services';
import { UiService } from './common/services/ui.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {

    public collapsed = true;

    constructor(
        account: AccountService,
        public ui: UiService
    ) {
        account.getInfo().catch(ex => {
            console.error('get account info with error: ', ex);
        });
    }

}
