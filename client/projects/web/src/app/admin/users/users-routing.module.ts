import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ListComponent } from './list/list.component';
import { DetailComponent } from './detail/detail.component';
import { LockComponent } from './lock/lock.component';
import { PasswordComponent } from './password/password.component';
import { RolesComponent } from './roles/roles.component';


const routes: Routes = [
    {
        path: '', component: ListComponent,
        children: [
            {
                path: ':id',
                component: DetailComponent
            },
            {
                path: ':id/lock',
                component: LockComponent
            },
            {
                path: ':id/password',
                component: PasswordComponent
            },
            {
                path: ':id/roles',
                component: RolesComponent
            }
        ]
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsersRoutingModule { }
