import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { XsrfGuard, AuthGuard } from 'services';

import { environment } from '../environments/environment';

const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    {
        path: 'home',
        loadChildren: './home/home.module#HomeModule',
        canLoad: [XsrfGuard]
    },
    {
        path: 'about',
        loadChildren: './about/about.module#AboutModule',
        canLoad: [XsrfGuard, AuthGuard]
    },
    {
        path: 'admin',
        // tslint:disable-next-line: max-line-length
        loadChildren: () => import('./admin/admin.module').then(m => m.AdminModule),
        canLoad: [XsrfGuard, AuthGuard],
        canActivate: [AuthGuard]
    },
    {
        path: 'login',
        // tslint:disable-next-line: max-line-length
        loadChildren: () => import('./login/login.module').then(m => m.LoginModule),
        canLoad: [XsrfGuard]
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
