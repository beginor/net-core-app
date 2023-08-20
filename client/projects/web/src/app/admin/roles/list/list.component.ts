import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap'

import { AccountService } from 'app-shared';
import { RolesService, AppRoleModel } from '../roles.service';
import { DetailComponent } from '../detail/detail.component';
import { PrivilegeComponent } from '../privilege/privilege.component';

@Component({
    selector: 'app-role-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit, OnDestroy {

    constructor(
        private offcanvas: NgbOffcanvas,
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: RolesService
    ) { }

    public ngOnInit(): void {
        void this.loadData();
    }

    public ngOnDestroy(): void {
        this.vm.cleanUp();
    }

    public async loadData(): Promise<void> {
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

    public showPrivileges(role: AppRoleModel): void {
        const ref = this.offcanvas.open(
            PrivilegeComponent,
            { position: 'end', panelClass: 'offcanvas-vw-40' }
        );
        const privilege = ref.componentInstance as PrivilegeComponent;
        privilege.id = role.id;
        privilege.title = `${role.description}权限列表`;
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

}
