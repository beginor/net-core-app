import { Component, OnInit } from '@angular/core';
import { NgbDate, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

import { AccountService, UserInfo } from 'app-shared';
import { UiService } from '..';

@Component({
    selector: 'app-account-user-info',
    templateUrl: './user-info.component.html',
    styleUrls: ['./user-info.component.scss']
})
export class UserInfoComponent implements OnInit {

    public user?: UserInfo;
    public dob?: NgbDate;
    public loading = false;
    public loadingMessage = '';

    constructor(
        public account: AccountService,
        private ui: UiService,
        private formatter: NgbDateParserFormatter
    ) { }

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

}
