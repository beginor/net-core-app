import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AuthGuard, isProd } from 'app-shared';

const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    {
        path: 'home',
        loadChildren: () => import('./home/home.module').then(m => m.HomeModule), // eslint-disable-line max-len
        canLoad: [AuthGuard],
        canActivate: [AuthGuard],
        data: { }
    },
    {
        path: 'about',
        loadChildren: () => import('./about/about.module').then(m => m.AboutModule), // eslint-disable-line max-len
        canLoad: [AuthGuard],
        canActivate: [AuthGuard],
        data: { }
    },
    {
        path: 'login',
        loadChildren: () => import('./login/login.module').then(m => m.LoginModule), // eslint-disable-line max-len
        canLoad: []
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, {
        useHash: false,
        enableTracing: isProd()
    })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
