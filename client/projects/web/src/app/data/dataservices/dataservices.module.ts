import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
    NgbPaginationModule, NgbTooltipModule, NgbTypeaheadModule, NgbModalModule,
    NgbNavModule, NgbProgressbarModule
} from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { AppCommonModule } from 'projects/web/src/app/common';
import { DataServiceRoutingModule } from './dataservices-routing.module';
import { ListComponent } from './list/list.component';
import { DetailComponent } from './detail/detail.component';
import { PreviewComponent } from './preview/preview.component';
import { PreviewDataComponent } from './preview/preview-data.component';
import { PreviewGeoJsonComponent } from './preview/preview-geojson.component';
import { PreviewFeatureSetComponent } from './preview/preview-featureset.component';
import { PreviewMapServerComponent } from './preview/preview-mapserver.component';

@NgModule({
    declarations: [
        ListComponent,
        DetailComponent,
        PreviewComponent,
        PreviewDataComponent,
        PreviewGeoJsonComponent,
        PreviewFeatureSetComponent,
        PreviewMapServerComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        NgbPaginationModule,
        NgbTooltipModule,
        NgbTypeaheadModule,
        NgbModalModule,
        NgbNavModule,
        NgbProgressbarModule,
        AppSharedModule,
        AppCommonModule,
        DataServiceRoutingModule
    ]
})
export class DataServiceModule { }
