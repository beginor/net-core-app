import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CanLoad, Route, UrlSegment } from '@angular/router';

import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class XsrfGuard implements CanLoad {

    constructor(private http: HttpClient) { }

    canLoad(route: Route, segments: UrlSegment[]): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            const url = environment.apiRoot + '/security/xsrf-token';
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
