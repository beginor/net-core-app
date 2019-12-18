import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AccountService } from 'app-shared';

@Component({
    selector: 'app-nav-topbar',
    templateUrl: './nav-topbar.component.html',
    styleUrls: ['./nav-topbar.component.scss']
})
export class NavTopBarComponent implements OnInit {

    public collapsed = true;

    constructor(
        private router: Router,
        public account: AccountService
    ) { }

    public ngOnInit(): void {
    }

    public async logout(e: Event): Promise<void> {
        e.preventDefault();
        await this.account.logout();
        await this.router.navigateByUrl('/');
    }

}
