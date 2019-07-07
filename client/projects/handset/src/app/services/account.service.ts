import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AccountService {

    public info = new BehaviorSubject<AccountInfo>({});

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string
    ) { }

    public async getInfo(): Promise<AccountInfo> {
        const url = this.apiRoot + '/account/info';
        const info = await this.http.get<AccountInfo>(url).toPromise();
        this.info.next(info);
        return info;
    }

    public async login(model: LoginModel): Promise<void> {
        const url = this.apiRoot + '/account';
        await this.http.post<any>(url, model).toPromise();
    }

    public async logout(): Promise<void> {
        const url = this.apiRoot + '/account';
        await this.http.delete(url).toPromise();
        this.info.next({});
    }

}

export interface AccountInfo {
    id?: string;
    userName?: string;
    givenName?: string;
    surename?: string;
    roles?: string[];
}

export interface LoginModel {
    userName?: string;
    password?: string;
    isPersistent?: boolean;
}
