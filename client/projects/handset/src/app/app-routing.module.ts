import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { environment } from '../environments/environment';
import { AuthGuard } from './services/auth.guard';
import { XsrfGuard } from './services/xsrf.guard';

const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    {
        path: 'home',
        loadChildren: './home/home.module#HomeModule',
        canLoad: [XsrfGuard, AuthGuard],
        canActivate: [AuthGuard],
        data: { }
    },
    {
        path: 'about',
        loadChildren: './about/about.module#AboutModule',
        canLoad: [XsrfGuard, AuthGuard],
        canActivate: [AuthGuard],
        data: { }
    },
    {
        path: 'login',
        loadChildren: './login/login.module#LoginModule',
        canLoad: [XsrfGuard]
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, {
        useHash: !environment.production,
        enableTracing: !environment.production
    })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
