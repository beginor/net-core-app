import { Component } from '@angular/core';

import { AccountService } from 'app-shared';
import { NavigationService } from '../../common';

@Component({
    selector: 'app-admin-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {

    constructor(
        public account: AccountService,
        public navigation: NavigationService
    ) { }

}
