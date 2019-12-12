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
    ) {
        const roleName = route.snapshot.params.roleName;
        if (!!roleName) {
            vm.searchModel.roleName = roleName;
        }
        else {
            vm.searchModel.roleName = '';
        }
    }

    public ngOnInit(): void {
        this.loadData();
    }

    public ngOnDestroy(): void {
        // this.vm.cleanUp();
    }

    public async loadData(): Promise<void> {
        await this.vm.getRoles();
        await this.vm.search();
    }

    public showDetail(id: string, editable: boolean): void {
        this.router.navigate(
            ['./', id, { editable: editable }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    public navigateTo(user: UserModel, page: string): void {
        this.router.navigate(
            ['./', user.id, page, { fullname: this.getFullname(user) }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

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

    public cleanUserSearch(): void {
        this.vm.searchModel.userName = '';
        this.vm.searchModel.skip = 0;
        this.vm.search();
    }

    public searchByRole(): void {
        this.vm.searchModel.skip = 0;
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

    public getUserCount(): number {
        const usersCount = this.vm.roles.getValue()
            .map(r => r.userCount || 0)
            .reduce((prev, curr) => prev + curr, 0);
        return usersCount;
    }

}
