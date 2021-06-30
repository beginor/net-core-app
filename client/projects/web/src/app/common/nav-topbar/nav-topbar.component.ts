import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { AccountService } from 'app-shared';
import { NavigationService } from '../services/navigation.service';

@Component({
    selector: 'app-nav-topbar',
    templateUrl: './nav-topbar.component.html',
    styleUrls: ['./nav-topbar.component.scss']
})
export class NavTopBarComponent {

    public collapsed = true;

    constructor(
        private router: Router,
        public account: AccountService,
        public navigation: NavigationService
    ) { }

    public async logout(e: Event): Promise<void> {
        e.preventDefault();
        await this.account.logout();
        await this.router.navigateByUrl('/');
    }

}
