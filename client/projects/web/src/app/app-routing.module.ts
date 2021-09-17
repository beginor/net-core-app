import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from 'app-shared';

import { environment } from '../environments/environment';
import { IframeComponent } from './common';
/* eslint-disable max-len */
const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    {
        path: 'home',
        loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
        canLoad: []
    },
    {
        path: 'about',
        loadChildren: () => import('./about/about.module').then(m => m.AboutModule),
        canLoad: [AuthGuard]
    },
    {
        path: 'admin',
        loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule),
        canLoad: [AuthGuard],
        canActivate: [AuthGuard]
    },
    {
        path: 'login',
        loadChildren: () => import('./login/login.module').then(m => m.LoginModule),
        canLoad: []
    },
    {
        path: 'iframe/:src',
        component: IframeComponent,
        canLoad: []
    },
    {
        path: 'account',
        loadChildren: () => import('./account/account.module').then(m => m.AccountModule),
        canLoad: [AuthGuard]
    }
];
/* eslint-enable max-len */
@NgModule({
    imports: [
        RouterModule.forRoot(
            routes,
            { useHash: false, enableTracing: !environment.production }
        )
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
