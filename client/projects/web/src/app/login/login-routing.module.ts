import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { XsrfGuard } from 'services';

import { LoginComponent } from './login/login.component';

const routes: Routes = [
    {
        path: '',
        component: LoginComponent,
        canActivate: [XsrfGuard]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class LoginRoutingModule { }
