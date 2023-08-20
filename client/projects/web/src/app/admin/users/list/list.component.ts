import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap'

import { AccountService } from 'app-shared';

import { UsersService, UserModel } from '../users.service';
import { DetailComponent } from '../detail/detail.component';
import { LockComponent } from '../lock/lock.component';
import { PasswordComponent } from '../password/password.component';
import { RolesComponent } from '../roles/roles.component';

@Component({
    selector: 'app-admin-users-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {

    constructor(
        route: ActivatedRoute,
        private offcanvas: NgbOffcanvas,
        public account: AccountService,
        public vm: UsersService
    ) {
        const { roleName } = route.snapshot.params;
        if (!!roleName) {
            vm.searchModel.roleName = roleName as string;
        }
        else {
            vm.searchModel.roleName = '';
        }
    }

    public ngOnInit(): void {
        void this.loadData();
    }

    public async loadData(): Promise<void> {
        await this.vm.getRoles();
        await this.vm.search();
    }

    public showDetail(id: string, editable: boolean): void {
        const ref = this.offcanvas.open(
            DetailComponent,
            { position: 'end', panelClass: 'offcanvas-vw-40' }
        );
        const detail = ref.componentInstance as DetailComponent;
        detail.editable = editable;
        detail.id = id;
        void ref.result.then(() => {
            void this.vm.search();
        }).catch(ex => {
            console.log(`offcanvas canceled with reason ${ex}`)
        });
    }

    public showLock(user: UserModel): void {
        const ref = this.offcanvas.open(
            LockComponent,
            { position: 'end', panelClass: 'offcanvas-vw-40' }
        );
        const lock = ref.componentInstance as LockComponent;
        lock.userId = user.id;
        lock.fullname = this.getFullname(user);
        lock.editable = true;
        void ref.result.then(() => {
            void this.vm.search();
        }).catch(ex => {
            console.log(`offcanvas canceled with reason ${ex}`)
        });
    }

    public showPassword(user: UserModel): void {
        const ref = this.offcanvas.open(
            PasswordComponent,
            { position: 'end', panelClass: 'offcanvas-vw-40' }
        );
        const lock = ref.componentInstance as PasswordComponent;
        lock.userId = user.id;
        lock.fullname = this.getFullname(user);
        lock.editable = true;
        void ref.result.then(() => {
            void this.vm.search();
        }).catch(ex => {
            console.log(`offcanvas canceled with reason ${ex}`)
        });
    }

    public showRoles(user: UserModel): void {
        const ref = this.offcanvas.open(
            RolesComponent,
            { position: 'end', panelClass: 'offcanvas-vw-40' }
        );
        const lock = ref.componentInstance as RolesComponent;
        lock.userId = user.id;
        lock.fullname = this.getFullname(user);
        lock.editable = true;
        void ref.result.then(() => {
            void this.vm.search();
        }).catch(ex => {
            console.log(`offcanvas canceled with reason ${ex}`)
        });
    }

    public async delete(id: string): Promise<void> {
        const deleted = await this.vm.delete(id);
        if (deleted) {
            void this.vm.search();
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
        void this.vm.search();
    }

    public research(): void {
        this.vm.searchModel.skip = 0;
        void this.vm.search();
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
