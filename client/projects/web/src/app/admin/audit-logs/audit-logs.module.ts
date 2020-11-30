import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { NgbDatepickerModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';
import { AuditLogsRoutingModule } from './audit-logs-routing.module';
import { ListComponent } from './list/list.component';

@NgModule({
    declarations: [ListComponent],
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        NgbDatepickerModule,
        NgbPaginationModule,
        AppSharedModule,
        AuditLogsRoutingModule
    ]
})
export class AuditLogsModule { }
