import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class XsrfService {

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string
    ) { }

    public async refresh(): Promise<void> {
        const url = this.apiRoot + '/security/xsrf-token';
        await this.http.get(url).toPromise();
    }
}
