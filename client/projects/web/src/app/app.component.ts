import { Component, ErrorHandler } from '@angular/core';

import { AccountService } from 'app-shared';
import { UiService } from './common/services/ui.service';
import { NavigationService } from './common/services/navigation.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent {

    constructor(
        account: AccountService,
        public ui: UiService,
        public navigation: NavigationService,
        errorHandler: ErrorHandler
    ) {
        account.getInfo().catch(ex => {
            errorHandler.handleError(ex);
        });
    }

}
