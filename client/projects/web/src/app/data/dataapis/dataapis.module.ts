import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
    NgbPaginationModule, NgbTooltipModule, NgbModalModule
} from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { AppCommonModule } from 'projects/web/src/app/common';
import { DataApiRoutingModule } from './dataapis-routing.module';
import { ListComponent } from './list/list.component';
import { DetailComponent } from './detail/detail.component';
import { StatementComponent } from './statement/statement.component';
import { ParamColsComponent } from './param-cols/param-cols.component';
import { DebugComponent } from './debug/debug.component';
import { PreviewComponent } from './preview/preview.component';
import { PreviewGeoJsonComponent } from './preview/preview-geojson.component';

@NgModule({
    declarations: [
        ListComponent,
        DetailComponent,
        StatementComponent,
        ParamColsComponent,
        DebugComponent,
        PreviewComponent,
        PreviewGeoJsonComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        NgbPaginationModule,
        NgbTooltipModule,
        NgbModalModule,
        AppSharedModule,
        AppCommonModule,
        DataApiRoutingModule
    ]
})
export class DataApiModule { }
