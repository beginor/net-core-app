import { Component, OnInit } from '@angular/core';
import { NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap'

import { slideInRight, slideOutRight, AccountService } from 'app-shared';
import { AppPrivilegeModel, AppPrivilegeService } from '../privileges.service';

@Component({
    selector: 'app-privilege-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.css'],
})
export class DetailComponent implements OnInit {

    public id = '';
    public get title(): string {
        let title = '';
        if (this.id === '0') {
            title = '新建系统权限';
        }
        else if (this.editable) {
            title = '编辑系统权限';
        }
        else {
            title = '查看系统权限';
        }
        return title;
    }
    public editable = true;
    public model: AppPrivilegeModel = { id: '', name: '' };

    constructor(
        private activeOffcanvas: NgbActiveOffcanvas,
        public account: AccountService,
        public vm: AppPrivilegeService
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
        this.activeOffcanvas.close('ok');
    }

}
