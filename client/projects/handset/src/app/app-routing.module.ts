import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { environment } from '../environments/environment';

import { AuthGuard } from 'app-shared';

const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    {
        path: 'home',
        loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
        canLoad: [AuthGuard],
        canActivate: [AuthGuard],
        data: { }
    },
    {
        path: 'about',
        loadChildren: () => import('./about/about.module').then(m => m.AboutModule),
        canLoad: [AuthGuard],
        canActivate: [AuthGuard],
        data: { }
    },
    {
        path: 'login',
        loadChildren: () => import('./login/login.module').then(m => m.LoginModule),
        canLoad: []
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, {
        useHash: false,
        enableTracing: !environment.production
    })],
    exports: [RouterModule]
})
export class AppRoutingModule { }
