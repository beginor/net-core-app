import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DashboardComponent } from './dashboard/dashboard.component';


const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    { path: 'dashboard', component: DashboardComponent },
    {
        path: 'users',
        // tslint:disable-next-line: max-line-length
        loadChildren: () => import('./users/users.module').then(m => m.UsersModule)
    },
    {
        path: 'roles',
        // tslint:disable-next-line: max-line-length
        loadChildren: () => import('./roles/roles.module').then(m => m.RolesModule)
    },
    {
        path: 'privileges',
        // tslint:disable-next-line: max-line-length
        loadChildren: () => import('./privileges/privileges.module').then(m => m.PrivilegesModule)
    },
    {
        path: 'audit-logs',
        // tslint:disable-next-line: max-line-length
        loadChildren: () => import('./audit-logs/audit-logs.module').then(m => m.AuditLogsModule)
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
