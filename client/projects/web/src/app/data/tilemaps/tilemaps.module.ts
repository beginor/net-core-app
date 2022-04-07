import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClipboardModule } from '@angular/cdk/clipboard';
import {
    NgbPaginationModule, NgbTooltipModule, NgbModalModule
} from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { AppCommonModule } from 'projects/web/src/app/common';
import { TileMapRoutingModule } from './tilemaps-routing.module';
import { ListComponent } from './list/list.component';
import { DetailComponent } from './detail/detail.component';
import { PreviewComponent } from './preview/preview.component';

@NgModule({
    declarations: [
        ListComponent,
        DetailComponent,
        PreviewComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ClipboardModule,
        NgbPaginationModule,
        NgbTooltipModule,
        NgbModalModule,
        AppSharedModule,
        AppCommonModule,
        TileMapRoutingModule
    ]
})
export class TileMapModule { }
