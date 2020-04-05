import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

import { BehaviorSubject, Subscription, interval } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AccountService {

    public info = new BehaviorSubject<AccountInfo>({ id: '' });

    public fullName = new BehaviorSubject<string>('匿名用户');

    public get token(): string {
        return localStorage.getItem(this.tokenKey);
    }

    private get tokenKey(): string {
        return `Bearer:${this.apiRoot}`;
    }

    private interval$: Subscription;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string
    ) {
        this.interval$ = interval(1000 * 60 * 5).subscribe(
            () => this.getInfo()
        );
    }

    public async getInfo(): Promise<AccountInfo> {
        try {
            const url = this.apiRoot + '/account/info';
            const info = await this.http.get<AccountInfo>(url).toPromise();
            if (!!info.token) {
                this.saveToken(info.token);
                delete info.token;
            }
            const currInfo = this.info.getValue();
            if (currInfo.id !== info.id) {
                this.info.next(info);
                const fullname = [];
                if (!!info.surname) {
                    fullname.push(info.surname);
                }
                if (!!info.givenName) {
                    fullname.push(info.givenName);
                }
                if (fullname.length === 0) {
                    fullname.push(info.userName);
                }
                this.fullName.next(fullname.join(''));
            }
            return info;
        }
        catch (ex) {
            localStorage.removeItem(this.tokenKey);
            throw new Error('Can not get account info!');
        }
    }

    public async login(model: LoginModel): Promise<void> {
        const url = this.apiRoot + '/account';
        const token = await this.http.post(url, model, { responseType: 'text' })
            .toPromise();
        this.saveToken(token);
    }

    public logout(): void {
        this.removeToken();
        this.info.next({});
    }

    private saveToken(token: string): void {
        localStorage.setItem(this.tokenKey, token);
    }

    private removeToken(): void {
        localStorage.removeItem(this.tokenKey);
    }

}

export interface AccountInfo {
    id?: string;
    userName?: string;
    givenName?: string;
    surname?: string;
    roles?: { [key: string]: boolean };
    privileges?: { [key: string]: boolean };
    token?: string;
}

export interface LoginModel {
    userName?: string;
    password?: string;
    isPersistent?: boolean;
}
