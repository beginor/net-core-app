import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ListComponent } from './list/list.component';
import { DetailComponent } from './detail/detail.component';
import { PrivilegeComponent } from './privilege/privilege.component';
import { UserComponent } from './user/user.component';


const routes: Routes = [
    {
        path: '', component: ListComponent,
        children: [
            {
                path: ':id',
                component: DetailComponent
            },
            {
                path: ':id/privileges',
                component: PrivilegeComponent
            },
            {
                path: ':id/users',
                component: UserComponent
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class RolesRoutingModule { }
