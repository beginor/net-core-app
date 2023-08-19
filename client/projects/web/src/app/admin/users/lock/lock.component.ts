import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import {
    NgbCalendar, NgbDateStruct, NgbDateParserFormatter, NgbActiveOffcanvas,
} from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';
import { UsersService } from '../users.service';

@Component({
    selector: 'app-user-lock',
    templateUrl: './lock.component.html',
    styleUrls: ['./lock.component.css'],
})
export class LockComponent {

    public editable = true;

    public lockForm: FormGroup;

    public userId = '';
    public fullname = '';
    public get title(): string {
        return `锁定 ${this.fullname || '用户'}`
    }


    public get lockoutEnd(): FormControl {
        return this.lockForm.get('lockoutEnd') as FormControl;
    }

    constructor(
        private activeOffcanvas: NgbActiveOffcanvas,
        private dateFormatter: NgbDateParserFormatter,
        public calendar: NgbCalendar,
        public account: AccountService,
        public vm: UsersService
    ) {
        const nextDay = calendar.getNext(calendar.getToday(), 'd', 1);
        this.lockForm  = new FormGroup({
            lockoutEnd: new FormControl(
                { value: nextDay, disabled: !this.editable },
                Validators.required
            )
        });
    }

    public cancel(): void {
        this.activeOffcanvas.dismiss('');
    }

    public save(): void {
        const value = this.lockoutEnd.value as NgbDateStruct;
        const date = this.dateFormatter.format(value);
        void this.vm.lockUser(this.userId, `${date} 23:59:59`).then(
            () => this.activeOffcanvas.close('ok')
        );
    }

}
