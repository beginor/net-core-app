import { Injectable, ErrorHandler } from '@angular/core';
import {
    CanLoad, CanActivate, Route, UrlSegment, Router, ActivatedRouteSnapshot,
    RouterStateSnapshot, NavigationCancel
} from '@angular/router';

import { first } from 'rxjs/operators';

import { AccountService } from './account.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanLoad, CanActivate {

    constructor(
        private router: Router,
        private accountSvc: AccountService,
        private errorHandler: ErrorHandler
    ) { }

    public async canLoad(
        route: Route,
        segments: UrlSegment[]
    ): Promise<boolean> {
        this.router.events.pipe(first(_ => _ instanceof NavigationCancel))
            .subscribe((event: NavigationCancel) => {
                this.redirectToLogin(event.url);
            });
        try {
            const info = await this.accountSvc.getInfo();
            return !!info.id;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            return false;
        }
    }

    public canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            this.accountSvc.info.subscribe(
                info => {
                    resolve(!!info.id);
                }
            );
        });
    }

    private redirectToLogin(returnUrl?: string): void {
        this.router.navigate(
            ['/login', { returnUrl }]
        );
    }

}
