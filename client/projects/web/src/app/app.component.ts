import { Component, ErrorHandler } from '@angular/core';

import {
    NgbTooltipConfig,
    NgbDropdownConfig,
    NgbModalConfig
} from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';
import { UiService } from './common/services/ui.service';
import { NavigationService } from './common/services/navigation.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    providers: [
        NgbTooltipConfig,
        NgbDropdownConfig
    ]
})
export class AppComponent {

    constructor(
        account: AccountService,
        public ui: UiService,
        public navigation: NavigationService,
        errorHandler: ErrorHandler,
        tooltip: NgbTooltipConfig,
        dropdown: NgbDropdownConfig,
        modal: NgbModalConfig
    ) {
        tooltip.container = 'body';
        dropdown.container = 'body';
        modal.container = 'body';
        account.getInfo().catch(ex => {
            errorHandler.handleError(ex);
        });
    }

}
