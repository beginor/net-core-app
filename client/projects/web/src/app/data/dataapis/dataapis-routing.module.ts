import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ListComponent } from './list/list.component';
import { DetailComponent } from './detail/detail.component';
import { StatementComponent } from "./statement/statement.component";

const routes: Routes = [
    {
        path: '', component: ListComponent,
        children: [
            {
                path: ':id',
                component: DetailComponent
            },
            {
                path: ':id/statement',
                component: StatementComponent
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DataApiRoutingModule { }
