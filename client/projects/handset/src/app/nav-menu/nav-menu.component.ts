import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { AccountService } from 'app-shared';

import { UiService } from '../services/ui.service';
import { NavMenuService } from './nav-menu.service';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {

    constructor(
        private router: Router,
        private ui: UiService,
        public vm: NavMenuService,
        public accountSvc: AccountService
    ) { }

    public toggleDrawer(): void {
        this.ui.drawer.subscribe(drawer => {
            drawer.close();
        });
    }

    public async logout(): Promise<void> {
        await this.accountSvc.logout();
        this.toggleDrawer();
        await this.router.navigate(['/login'], { replaceUrl: true });
    }

}
