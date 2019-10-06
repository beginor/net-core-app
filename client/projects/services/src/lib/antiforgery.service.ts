import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class AntiforgeryService {

    private url: string;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') apiRoot: string
    ) {
        this.url = `${apiRoot}/security/antiforgery`;
    }

    public async refresh(): Promise<void> {
        await this.http.get(
            this.url,
            { responseType: 'text', observe: 'response' }
        ).toPromise();
    }
}
