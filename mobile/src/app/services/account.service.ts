import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { environment } from '../../environments/environment';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AccountService {

    info = new BehaviorSubject<AccountInfo>({});

    constructor(
        private http: HttpClient
    ) { }

    async getInfo(): Promise<AccountInfo> {
        const url = environment.apiRoot + '/account/info';
        const info = await this.http.get<AccountInfo>(url).toPromise();
        this.info.next(info);
        return info;
    }

    async login(model: LoginModel): Promise<void> {
        const url = environment.apiRoot + '/account';
        await this.http.post<any>(url, model).toPromise();
    }

    async logout(): Promise<void> {
        const url = environment.apiRoot + '/account';
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
