import { Component } from '@angular/core';
import { Router } from '@angular/router';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';
import { NavigationService } from '../services/navigation.service';
import { AccountComponent } from '../account/account.component';

@Component({
    selector: 'app-nav-topbar',
    templateUrl: './nav-topbar.component.html',
    styleUrls: ['./nav-topbar.component.scss']
})
export class NavTopBarComponent {

    public collapsed = true;

    constructor(
        private router: Router,
        private modal: NgbModal,
        public account: AccountService,
        public navigation: NavigationService
    ) { }

    public async logout(e: Event): Promise<void> {
        this.collapsed = true;
        e.preventDefault();
        await this.account.logout();
        await this.router.navigateByUrl('/');
    }

    public async showAccountInfoModal(e: MouseEvent): Promise<void> {
        e.preventDefault();
        this.modal.open(
            AccountComponent,
            { size: 'lg', container: 'body', backdrop: 'static' }
        );
    }

}
