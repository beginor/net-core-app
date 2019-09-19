import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
    CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot
} from '@angular/router';

@Injectable({
    providedIn: 'root'
})
export class XsrfGuard implements CanActivate {

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string
    ) { }

    public async canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Promise<boolean> {
        try {
            await this.refresh();
            return true;
        }
        catch (ex) {
            return false;
        }
    }

    public async refresh(): Promise<void> {
        const url = this.apiRoot + '/security/xsrf-token';
        await this.http.get(url).toPromise();
    }
}
