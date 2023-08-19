import { Component } from '@angular/core';
import {
    FormGroup, FormControl, Validators
} from '@angular/forms';
import { NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap';

import {
    AccountService, confirmTo
} from 'app-shared';
import { UsersService } from '../users.service';

@Component({
    selector: 'app-user-password',
    templateUrl: './password.component.html',
    styleUrls: ['./password.component.css'],
})
export class PasswordComponent {

    public userId = '';
    public fullname = '';
    public get title(): string {
        return `重置 ${this.fullname} 的密码`;
    }
    public editable = true;
    public form: FormGroup;

    public get password(): FormControl {
        return this.form.get('password') as FormControl;
    }

    public get confirmPassword(): FormControl {
        return this.form.get('confirmPassword') as FormControl;
    }

    constructor(
        private activeOffcanvas: NgbActiveOffcanvas,
        public account: AccountService,
        public vm: UsersService
    ) {
        this.form = new FormGroup({
            password: new FormControl(
                { value: '', disabled: !this.editable },
                [ Validators.required, Validators.minLength(8) ]
            ),
            confirmPassword: new FormControl(
                { value: '', disabled: !this.editable },
                [ Validators.required, confirmTo('password') ]
            )
        });
    }

    public cancel(): void {
        this.activeOffcanvas.dismiss('');
    }

    public async save(): Promise<void> {
        await this.vm.resetPass(this.userId, this.form.value);
        this.activeOffcanvas.close('ok')
    }

}
