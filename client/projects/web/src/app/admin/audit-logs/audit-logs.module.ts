import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';

import { AuditLogsRoutingModule } from './audit-logs-routing.module';
import { ListComponent } from './list/list.component';


@NgModule({
    declarations: [ListComponent],
    imports: [
        CommonModule,
        HttpClientModule,
        NgbDatepickerModule,
        AuditLogsRoutingModule
    ]
})
export class AuditLogsModule { }
