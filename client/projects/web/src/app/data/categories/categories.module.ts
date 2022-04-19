import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DragDropModule } from '@angular/cdk/drag-drop';

import {
    NgbPaginationModule, NgbTooltipModule, NgbPopoverModule
} from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { AppCommonModule } from 'projects/web/src/app/common';
import { CategoryRoutingModule } from './categories-routing.module';
import { TreeComponent } from './tree/tree.component';
import { TreeItemComponent } from './tree-item/tree-item.component';
import {
    TreeItemEditComponent
} from './tree-item-edit/tree-item-edit.component';

@NgModule({
    declarations: [
        TreeComponent,
        TreeItemComponent,
        TreeItemEditComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        DragDropModule,
        NgbPaginationModule,
        NgbTooltipModule,
        NgbPopoverModule,
        AppSharedModule,
        AppCommonModule,
        CategoryRoutingModule
    ]
})
export class CategoryModule { }
