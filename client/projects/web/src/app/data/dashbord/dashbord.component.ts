import { Component } from '@angular/core';

import { AccountService } from 'app-shared';
import { NavigationService } from 'projects/web/src/app/common';

@Component({
    selector: 'app-dashbord',
    templateUrl: './dashbord.component.html',
    styleUrls: ['./dashbord.component.scss']
})
export class DashbordComponent {

    constructor(
        public account: AccountService,
        public navigation: NavigationService
    ) { }

}
