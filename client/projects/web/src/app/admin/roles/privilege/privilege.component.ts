import { Component, OnInit } from '@angular/core';
import { NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap';

import { RolesService } from '../roles.service';

@Component({
    selector: 'app-role-privilege',
    templateUrl: './privilege.component.html',
    styleUrls: ['./privilege.component.css'],
})
export class PrivilegeComponent implements OnInit {

    public title = '';
    public id = '';

    constructor(
        private activeOffcanvas: NgbActiveOffcanvas,
        public vm: RolesService
    ) { }

    public ngOnInit(): void {
        this.vm.getPrivilegesForRole(this.id);
        this.vm.getAllPrivileges();
    }

    public cancel(): void {
        this.activeOffcanvas.dismiss('');
    }

    public async togglePrivilege(e: Event): Promise<void> {
        e.preventDefault();
        e.stopPropagation();
        const checkbox = e.target as HTMLInputElement;
        const privilege = checkbox.value;
        await this.vm.toggleRolePrivilege(this.id, privilege);
    }

}
