import { Component, Input, OnInit } from '@angular/core';
import { AccountService } from 'app-shared';

import { RolesService } from '../../admin/roles/roles.service';

@Component({
    selector: 'app-roles-editor',
    templateUrl: './roles-editor.component.html',
    styleUrls: ['./roles-editor.component.scss']
})
export class RolesEditorComponent implements OnInit {
    
    private _roles: string[] = [];
    
    public get roles(): string[] { return this._roles; }
    @Input()
    public set roles(val: string[]) {
        this._roles = val;
        if (!val) {
            return;
        }
        if (val.length === 0) {
            this.addDefaultRoles();
        }
    }
    @Input()
    public editable: boolean = true;

    constructor(
        private account: AccountService,
        public vm: RolesService
    ) {}
    
    public async ngOnInit(): Promise<void> {
        this.vm.searchModel.skip = 0;
        this.vm.searchModel.take = 999;
        await this.vm.search();
        if (!this.roles) {
            return;
        }
        if (this.roles.length == 0) {
            this.addDefaultRoles();
        }
    }
    
    public isRoleChecked(role: string): boolean {
        if (!this.roles) {
            return false;
        }
        return this.roles.indexOf(role) > -1;
    }

    public toggleCheckedRole(role: string): void {
        const idx = this.roles.indexOf(role);
        if (idx > -1) {
            this.roles.splice(idx, 1);
        }
        else {
            this.roles.push(role);
        }
    }
    
    private addDefaultRoles(): void {
        this.vm.data.getValue()
            .filter(role => !!role.isDefault)
            .map(role => role.name)
            .forEach(role => {
                this._roles.push(role);
            });
    }

}
