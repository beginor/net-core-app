import { Component, OnInit } from '@angular/core';

import { AccountService } from 'app-shared';

@Component({
    selector: 'app-admin-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

    constructor(
        public account: AccountService
    ) { }

    public ngOnInit(): void {
    }

}
