import { Component } from '@angular/core';

import { AccountService } from 'app-shared';

@Component({
    selector: 'app-dashbord',
    templateUrl: './dashbord.component.html',
    styleUrls: ['./dashbord.component.scss']
})
export class DashbordComponent {

    constructor(
        public account: AccountService
    ) { }

}
