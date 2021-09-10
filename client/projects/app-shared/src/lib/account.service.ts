import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

import { BehaviorSubject, Subscription, interval, lastValueFrom } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AccountService {

    public info = new BehaviorSubject<AccountInfo>(
        { id: '', userName: 'anonymous', roles: { }, privileges: { } }
    );

    public fullName = new BehaviorSubject<string>('匿名用户');

    public get token(): string {
        return localStorage.getItem(this.tokenKey) as string;
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
            () => void this.getInfo()
        );
    }

    public async getInfo(): Promise<AccountInfo> {
        try {
            const url = `${this.apiRoot}/account`;
            const info = await lastValueFrom(this.http.get<AccountInfo>(url));
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
        const loginModel: LoginModel = {
            userName: btoa(model.userName as string),
            password: btoa(model.password as string),
            isPersistent: model.isPersistent
        };
        const token = await lastValueFrom(
            this.http.post(url, loginModel, { responseType: 'text' })
        );
        this.saveToken(token);
    }

    public logout(): void {
        this.removeToken();
        this.info.next({ id: '', roles: {}, privileges: {} });
    }

    public async getUser(): Promise<UserInfo> {
        const params = new HttpParams().set('_t', Date.now());
        const userInfo = await lastValueFrom(
            this.http.get<UserInfo>(
                `${this.apiRoot}/account/user`,
                { params, }
            )
        );
        return userInfo;
    }

    public async updateUser(userInfo: UserInfo): Promise<UserInfo> {
        const updatedUserInfo = await lastValueFrom(
            this.http.put<UserInfo>(
                `${this.apiRoot}/account/user`,
                userInfo
            )
        );
        return updatedUserInfo;
    }

    private saveToken(token: string): void {
        localStorage.setItem(this.tokenKey, token);
    }

    private removeToken(): void {
        localStorage.removeItem(this.tokenKey);
    }

}

export interface AccountInfo {
    id: string;
    userName?: string;
    givenName?: string;
    surname?: string;
    roles: { [key: string]: boolean };
    privileges: { [key: string]: boolean };
    token?: string;
}

export interface UserInfo {
    /** 用户ID */
    id: string;
    /** 用户名 */
    userName?: string;
    /** 电子邮箱地址 */
    email?: string;
    /** 电子邮箱地址是否已确认 */
    emailConfirmed?: boolean;
    /** 电话号码 */
    phoneNumber?: string;
    /** 电话号码是否已经确认 */
    phoneNumberConfirmed?: boolean;
    /** 是否允许（自动）锁定 */
    lockoutEnabled?: boolean;
    /** 锁定结束时间 */
    lockoutEnd?: string;
    /** 登录失败次数 */
    accessFailedCount?: number;
    /** 是否启用两部认证 */
    twoFactorEnabled?: boolean;
    /** 创建时间 */
    createTime?: string;
    /** 最近登录时间 */
    lastLogin?: string;
    /** 登录次数 */
    loginCount?: number;
    /** 姓氏 */
    surname?: string;
    /** 名称 */
    givenName?: string;
    /** 出生日期 */
    dateOfBirth?: string;
    /** 性别 */
    gender?: string;
    /** 家庭地址 */
    streetAddress?: string;
}

export interface LoginModel {
    userName?: string;
    password?: string;
    isPersistent?: boolean;
}
