import { Injectable, ErrorHandler } from '@angular/core';
import {
    Route, UrlSegment, Router, ActivatedRouteSnapshot,
    RouterStateSnapshot, NavigationError,
} from '@angular/router';

import { first } from 'rxjs/operators';

import { AccountService } from './account.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard {

    constructor(
        private router: Router,
        private accountSvc: AccountService,
        private errorHandler: ErrorHandler
    ) {
        this.router.events.pipe(first(evt => evt instanceof NavigationError))
            .subscribe(e => {
                const nc = e as NavigationError;
                this.redirectToLogin(nc.url);
            });
    }

    public async canLoad(
        route: Route,
        segments: UrlSegment[]
    ): Promise<boolean> {
        try {
            const info = await this.accountSvc.getInfo();
            return !!info.id;
        }
        catch (ex: unknown) {
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
        void this.router.navigate(
            ['/login', { returnUrl }]
        );
    }

}
