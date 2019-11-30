import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AccountService } from 'app-shared';

import { UsersService, UserModel } from '../users.service';

@Component({
    selector: 'app-admin-users-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit, OnDestroy {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: UsersService
    ) { }

    public ngOnInit(): void {
        this.loadData();
    }

    public ngOnDestroy(): void {
        // this.vm.cleanUp();
    }

    public async loadData(): Promise<void> {
        await this.vm.search();
    }

    public showDetail(id: string, editable: boolean): void {
        this.router.navigate(
            ['./', id, { editable: editable }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    // public showUsers(role: UserModel): void {
    //     this.router.navigate(
    //         ['./', role.id, 'users', { desc: role.description }],
    //         { relativeTo: this.route, skipLocationChange: true }
    //     );
    // }

    // public showPrivileges(role: UserModel): void {
    //     this.router.navigate(
    //         ['./', role.id, 'privileges', { desc: role.description }],
    //         { relativeTo: this.route, skipLocationChange: true }
    //     );
    // }

    public async delete(id: string): Promise<void> {
        const deleted = await this.vm.delete(id);
        if (deleted) {
            this.vm.search();
        }
    }

    public getFullname(user: UserModel): string {
        const fullname = [];
        if (!!user.surname) {
            fullname.push(user.surname);
        }
        if (!!user.givenName) {
            fullname.push(user.givenName);
        }
        if (fullname.length > 0) {
            fullname.push('(');
        }
        fullname.push(user.userName);
        if (fullname.length > 1) {
            fullname.push(')');
        }
        return fullname.join('');
    }

    public cleanSearch(): void {
        this.vm.searchModel.userName = '';
        this.vm.search();
    }

    public isLockout(user: UserModel): boolean {
        if (!user.lockoutEnabled) {
            return false;
        }
        if (!user.lockoutEnd) {
            return false;
        }
        else {
            const lockoutEnd = new Date(user.lockoutEnd);
            return lockoutEnd > new Date();
        }
    }

    public canViewGears(): boolean {
        const p = this.account.info.getValue().privileges;
        if (!p) {
            return false;
        }
        return p['app_users.update'] || p['app_users.delete']
            || p['app_users.reset_pass'] || p['app_users.lock']
            || p['app_users.unlock'] || p['app_users.read_user_roles'];
    }

}
