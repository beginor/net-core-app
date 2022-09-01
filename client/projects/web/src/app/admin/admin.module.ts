import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AppSharedModule } from 'app-shared';
import { AppCommonModule } from '../common/app-common.module';

import { AdminRoutingModule } from './admin-routing.module';
import { DashboardComponent } from './dashboard/dashboard.component';


@NgModule({
    declarations: [DashboardComponent],
    imports: [
        CommonModule,
        AppSharedModule,
        AppCommonModule,
        AdminRoutingModule
    ]
})
export class AdminModule { }
