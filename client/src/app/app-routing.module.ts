import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { environment } from '../environments/environment';

const routes: Routes = [
    { path: '', redirectTo: '/home', pathMatch: 'full' },
    { path: 'home', loadChildren: './home/home.module#HomeModule' },
    { path: 'about', loadChildren: './about/about.module#AboutModule' }
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
