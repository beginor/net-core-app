import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CanLoad, Route, UrlSegment } from '@angular/router';

@Injectable({
    providedIn: 'root'
})
export class XsrfGuard implements CanLoad {

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string
    ) { }

    public canLoad(route: Route, segments: UrlSegment[]): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            const url = this.apiRoot + '/security/xsrf-token';
            this.http.get(url).toPromise()
                .then(() => {
                    resolve(true);
                })
                .catch(() => {
                    reject(new Error('Can not get xsrf token'));
                });
        });
    }

}
