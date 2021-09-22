import { Injectable } from '@angular/core';

import {
    AccountService, UserTokenModel, UserTokenSearchModel
} from 'app-shared';
import { UiService } from '../common';

@Injectable({
    providedIn: 'root'
})
export class TokenService {

    public total = 0;
    public tokens: UserTokenModel[] = [];

    public loading = false;
    public model: UserTokenSearchModel = { skip: 0, take: 100 };

    constructor(
        private account: AccountService,
        private ui: UiService
    ) { }

    public async loadTokens(): Promise<void> {
        try {
            this.loading = true;
            const result = await this.account.searchUserTokens(this.model);
            this.total = result.total ?? 0;
            this.tokens = result.data ?? [];
        }
        catch (ex) {
            this.ui.showAlert(
                { type: 'danger', message: '加载凭证列表出错！' }
            );
        }
        finally {
            this.loading = false;
        }
    }


}
