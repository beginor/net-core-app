import { Component } from '@angular/core';

import { AccountService } from 'app-shared';

@Component({
    selector: 'app-admin-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {

    constructor(
        public account: AccountService
    ) { }

}
