import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AccountService, AntiforgeryService } from 'services';

@Component({
    selector: 'app-nav-bar',
    templateUrl: './nav-bar.component.html',
    styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {

    public collapsed = true;

    constructor(
        private router: Router,
        public account: AccountService,
        private antiforgery: AntiforgeryService
    ) { }

    public ngOnInit(): void {
    }

    public async logout(e: Event): Promise<void> {
        e.preventDefault();
        await this.antiforgery.refresh();
        await this.account.logout();
        await this.router.navigateByUrl('/');
    }

}
