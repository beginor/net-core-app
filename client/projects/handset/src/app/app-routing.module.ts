import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { environment } from '../environments/environment';

import { AuthGuard } from 'app-shared';

const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    {
        path: 'home',
        loadChildren: './home/home.module#HomeModule',
        canLoad: [AuthGuard],
        canActivate: [AuthGuard],
        data: { }
    },
    {
        path: 'about',
        loadChildren: './about/about.module#AboutModule',
        canLoad: [AuthGuard],
        canActivate: [AuthGuard],
        data: { }
    },
    {
        path: 'login',
        loadChildren: './login/login.module#LoginModule',
        canLoad: []
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
