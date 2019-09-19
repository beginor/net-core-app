import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard, XsrfGuard } from 'services';

import { environment } from '../environments/environment';

const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    {
        path: 'home',
        loadChildren: './home/home.module#HomeModule',
        canLoad: []
    },
    {
        path: 'about',
        loadChildren: './about/about.module#AboutModule',
        canLoad: [AuthGuard]
    },
    {
        path: 'admin',
        // tslint:disable-next-line: max-line-length
        loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule),
        canLoad: [AuthGuard],
        canActivate: [AuthGuard]
    },
    {
        path: 'login',
        // tslint:disable-next-line: max-line-length
        loadChildren: () => import('./login/login.module').then(m => m.LoginModule),
        canLoad: []
    }
];

@NgModule({
    imports: [
        RouterModule.forRoot(routes, {
            useHash: !environment.production,
            enableTracing: !environment.production
        })
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
