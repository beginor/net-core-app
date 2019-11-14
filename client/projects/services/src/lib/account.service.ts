import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';

import { BehaviorSubject, Subscription, interval } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AccountService {

    public info = new BehaviorSubject<AccountInfo>({});
    public get token(): string {
        return localStorage.getItem(`Bearer:${this.apiRoot}`);
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
            const currInfo = this.info.getValue();
            if (currInfo.id !== info.id) {
                this.info.next(info);
            }
            return info;
        }
        catch (ex) {
            localStorage.removeItem(`Bearer:${this.apiRoot}`);
            throw new Error('Can not get account info!');
        }
    }

    public async login(model: LoginModel): Promise<void> {
        const url = this.apiRoot + '/account';
        const token = await this.http.post(url, model, { responseType: 'text' })
            .toPromise();
        localStorage.setItem(`Bearer:${this.apiRoot}`, token);
    }

    public logout(): void {
        localStorage.removeItem(`Bearer:${this.apiRoot}`);
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
