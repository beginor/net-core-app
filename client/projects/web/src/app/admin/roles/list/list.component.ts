import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AccountService } from 'app-shared';
import { RolesService, AppRoleModel } from '../roles.service';

@Component({
    selector: 'app-role-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit, OnDestroy {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: RolesService
    ) { }

    public ngOnInit(): void {
        this.loadData();
    }

    public ngOnDestroy(): void {
        this.vm.cleanUp();
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

    public showUsers(role: AppRoleModel): void {
        this.router.navigate(
            ['./', role.id, 'users', { desc: role.description }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    public showPrivileges(role: AppRoleModel): void {
        this.router.navigate(
            ['./', role.id, 'privileges', { desc: role.description }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    public async delete(id: string): Promise<void> {
        const deleted = await this.vm.delete(id);
        if (deleted) {
            this.vm.search();
        }
    }

}
