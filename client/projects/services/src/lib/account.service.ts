import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

import { BehaviorSubject, Subscription, interval } from 'rxjs';

import { XsrfService } from './xsrf.service';

@Injectable({
    providedIn: 'root'
})
export class AccountService {

    public info = new BehaviorSubject<AccountInfo>({});
    private interval$: Subscription;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private xsrf: XsrfService
    ) {
        this.interval$ = interval(1000 * 60).subscribe(
            () => this.getInfo()
        );
    }

    public async getInfo(): Promise<AccountInfo> {
        try {
            const url = this.apiRoot + '/account/info';
            const info = await this.http.get<AccountInfo>(url).toPromise();
            const currInfo = this.info.getValue();
            if (currInfo.id !== info.id) {
                this.info.next(info);
            }
            return info;
        }
        catch (ex) {
            throw new Error('Can not get account info!');
        }
    }

    public async login(model: LoginModel): Promise<void> {
        await this.xsrf.refresh();
        const url = this.apiRoot + '/account';
        await this.http.post<any>(url, model).toPromise();
    }

    public async logout(): Promise<void> {
        const url = this.apiRoot + '/account';
        await this.xsrf.refresh();
        await this.http.delete(url).toPromise();
        this.info.next({});
    }

}

export interface AccountInfo {
    id?: string;
    userName?: string;
    givenName?: string;
    surename?: string;
    roles?: { [key: string]: boolean };
    privileges?: { [key: string]: boolean };
}

export interface LoginModel {
    userName?: string;
    password?: string;
    isPersistent?: boolean;
}
