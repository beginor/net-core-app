import { Component, OnInit } from '@angular/core';
import { NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap';

import { RolesService, AppRoleModel } from '../roles.service';

@Component({
    selector: 'app-role-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.css'],
})
export class DetailComponent implements OnInit {

    public id = '';
    public get title(): string {
        let title = '';
        if (this.id === '0') {
            title = '新建角色';
        }
        else if (this.editable) {
            title = '编辑角色';
        }
        else {
            title = '查看角色';
        }
        return title;
    }
    public editable = true;

    public model: AppRoleModel = { id: '', name: '' };

    constructor(
        private activeOffcanvas: NgbActiveOffcanvas,
        public vm: RolesService
    ) { }

    public async ngOnInit(): Promise<void> {
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                this.model = model;
            }
        }
    }

    public cancel(): void {
        this.activeOffcanvas.dismiss('');
    }

    public async save(): Promise<void> {
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.activeOffcanvas.close('ok')
    }

}
