import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { environment } from '../environments/environment';
import { XsrfGuard } from './services/xsrf.guard';

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
        canLoad: [XsrfGuard]
    }
];

@NgModule({
    imports: [
        RouterModule.forRoot(routes, {
            useHash: false,
            enableTracing: !environment.production
        })
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
