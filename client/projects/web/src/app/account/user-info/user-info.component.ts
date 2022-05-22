import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder, FormControl } from '@angular/forms';
import { NgbDate, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

import {
    AccountService, UserInfo, ChangePasswordModel, confirmTo
} from 'app-shared';
import { UiService } from '../../common';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent implements OnInit {

    public user: UserInfo = { id: '' };
    public dob?: NgbDate;
    public loading = false;
    public loadingMessage = '';
    public updatingPwd = false;

    public pwdForm: FormGroup;

    public get currentPassword(): FormControl {
        return this.pwdForm.get('currentPassword') as FormControl;
    }
    public get newPassword(): FormControl {
        return this.pwdForm.get('newPassword') as FormControl;
    }
    public get confirmPassword(): FormControl {
        return this.pwdForm.get('confirmPassword') as FormControl;
    }

    constructor(
        fb: FormBuilder,
        public account: AccountService,
        private ui: UiService,
        private formatter: NgbDateParserFormatter
    ) {
        this.pwdForm = fb.group({
            currentPassword: fb.control(
                { value: '', disabled: false },
                [Validators.required]
            ),
            newPassword: fb.control(
                { value: '', disabled: false},
                [Validators.required, Validators.minLength(8)]
            ),
            confirmPassword: fb.control(
                { value: '', disabled: false },
                [Validators.required, confirmTo('newPassword')]
            )
        });
    }

    public async ngOnInit(): Promise<void> {
        try {
            this.loading = true;
            this.loadingMessage = '正在加载用户信息 ...';
            const user = await this.account.getUser();
            this.user = user;
            this.dob = NgbDate.from(
                this.formatter.parse(user.dateOfBirth as string)
            ) as NgbDate;
        }
        catch (ex: unknown) {
            this.ui.showAlert(
                { type: 'danger', message: '无法获取用户信息！' }
            );
        }
        finally {
            this.loading = false;
        }
    }

    public async saveUser(): Promise<void> {
        if (!this.user) {
            return;
        }
        try {
            this.loading = true;
            this.loadingMessage = '正在更新用户信息 ...';
            this.user.dateOfBirth = this.formatter.format(this.dob as NgbDate);
            var user = await this.account.updateUser(this.user);
            this.user = user;
        }
        catch (ex: unknown) {
            this.ui.showAlert(
                { type: 'danger', message: '无法更新用户信息！' }
            );
        }
        finally {
            this.loading = false;
        }
    }

    public async changePassword(): Promise<void> {
        try {
            this.updatingPwd = true;
            await this.account.changePassword(this.pwdForm.value);
            this.ui.showAlert(
                { type: 'success', message: '修改密码成功！' }
            );
            this.pwdForm.reset();
        }
        catch(ex) {
            this.ui.showAlert(
                { type: 'danger', message: '修改密码出错！' }
            );
        }
        finally {
            this.updatingPwd = false;
        }
    }

}
