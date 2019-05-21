import { Injectable } from '@angular/core';
import {
    CanLoad, CanActivate, Route, UrlSegment, Router, ActivatedRouteSnapshot,
    RouterStateSnapshot
} from '@angular/router';

import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanLoad, CanActivate {

    constructor(
        private router: Router,
        private svc: AccountService
    ) { }

    async canLoad(route: Route, segments: UrlSegment[]): Promise<boolean> {
        try {
            const info = await this.svc.getInfo();
            return !!info;
        }
        catch (ex) {
            this.router.navigate(
                ['/login', { returnUrl: route.path }]
            );
            console.error(ex);
            return false;
        }
    }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            this.svc.info.subscribe(
                info => {
                    resolve(!!info.id);
                }
            );
        });
    }

}
