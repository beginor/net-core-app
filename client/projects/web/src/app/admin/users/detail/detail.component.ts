import { Component, OnInit } from '@angular/core';
import { NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';
import { UserModel, UsersService} from '../users.service';

@Component({
    selector: 'app-users-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.css'],
})
export class DetailComponent implements OnInit {

    public editable = false;
    public id = '';

    public getTitle(): string {
        if (this.id === '0') {
            return '新建菜单项';
        }
        else if (this.editable) {
            return '编辑菜单项';
        }
        else {
            return '查看菜单项';
        }
    }

    public model: UserModel = { id: '', lockoutEnabled: true, gender: '保密' };
    public dob = { year: 1970, month: 1, day: 1 };

    constructor(
        private activeOffcanvas: NgbActiveOffcanvas,
        public account: AccountService,
        public vm: UsersService
    ) { }

    public async ngOnInit(): Promise<void> {
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                this.model = model;
            }
            if (!!this.model.dateOfBirth) {
                const dob = this.model.dateOfBirth.split('-');
                this.dob = {
                    year: parseInt(dob[0], 10),
                    month: parseInt(dob[1], 10),
                    day: parseInt(dob[2], 10)
                };
            }
        }
    }

    public cancel(): void {
        this.activeOffcanvas.dismiss('');
    }

    public async save(): Promise<void> {
        const dob = `${this.dob.year}-${this.dob.month}-${this.dob.day}`;
        this.model.dateOfBirth = dob;
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.activeOffcanvas.close('ok');
    }

}
