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
        catch (ex: unknown) {
            this.ui.showAlert(
                { type: 'danger', message: '加载凭证列表出错！' }
            );
        }
        finally {
            this.loading = false;
        }
    }

    public getById(id: string): UserTokenModel | undefined {
        const token = this.tokens.find(t => t.id === id);
        if (!token) {
            return;
        }
        return Object.assign({}, token);
    }

    public async create(model: UserTokenModel): Promise<void> {
        try {
            await this.account.createUserToken(model);
        }
        catch (ex: unknown) {
            this.ui.showAlert(
                { type: 'danger', message: '创建凭证出错！' }
            );
        }
    }

    public async update(id: string, model: UserTokenModel): Promise<void> {
        try {
            await this.account.updateUserToken(id, model);
        }
        catch (ex: unknown) {
            this.ui.showAlert(
                { type: 'danger', message: '更新凭证出错！' }
            );
        }
    }

}
