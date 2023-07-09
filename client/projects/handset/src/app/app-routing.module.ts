import {inject, NgModule} from '@angular/core';
import {
    Routes, RouterModule, Route, UrlSegment, ActivatedRouteSnapshot,
    RouterStateSnapshot
} from '@angular/router';

import { AuthGuard, isProd } from 'app-shared';

/* eslint-disable max-len */
const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    {
        path: 'home',
        loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
        canMatch: [
            (route: Route, segments: UrlSegment[]): Promise<boolean> => {
                return inject(AuthGuard).canLoad(route, segments);
            }
        ],
        canActivate: [
            (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> => {
                return inject(AuthGuard).canActivate(route, state);
            }
        ],
        data: { },
    },
    {
        path: 'about',
        loadChildren: () => import('./about/about.module').then(m => m.AboutModule), // eslint-disable-line max-len
        canMatch: [
            (route: Route, segments: UrlSegment[]): Promise<boolean> => {
                return inject(AuthGuard).canLoad(route, segments);
            }
        ],
        canActivate: [
            (route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> => {
                return inject(AuthGuard).canActivate(route, state);
            }
        ],
        data: { }
    },
    {
        path: 'login',
        loadChildren: () => import('./login/login.module').then(m => m.LoginModule),
        canMatch: []
    }
];
/* eslint-enable max-len */
@NgModule({
    imports: [RouterModule.forRoot(routes, {
        useHash: false,
        enableTracing: isProd()
    })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
