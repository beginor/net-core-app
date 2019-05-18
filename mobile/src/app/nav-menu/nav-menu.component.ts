import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { UiService } from '../services/ui.service';
import { NavMenuService } from './nav-menu.service';
import { AccountService } from '../services/account.service';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {

    constructor(
        private router: Router,
        private ui: UiService,
        public vm: NavMenuService,
        public accountSvc: AccountService
    ) { }

    ngOnInit(): void {
    }

    toggleDrawer(): void {
        this.ui.drawer.subscribe(drawer => {
            drawer.close();
        });
    }

    async logout(): Promise<void> {
        await this.accountSvc.logout();
        this.toggleDrawer();
        await this.router.navigate(['/login'], { replaceUrl: true });
    }

}
