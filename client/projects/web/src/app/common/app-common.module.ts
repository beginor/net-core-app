import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LayoutModule } from '@angular/cdk/layout';
import { ScrollingModule } from '@angular/cdk/scrolling';
import {
    NgbAlertModule, NgbDropdownModule, NgbModalModule, NgbTooltipModule,
    NgbCollapseModule, NgbNavModule, NgbDatepickerModule
} from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { NavTopBarComponent } from './nav-topbar/nav-topbar.component';
import { ConfirmComponent } from './confirm/confirm.component';
import { NavSidebarComponent } from './nav-sidebar/nav-sidebar.component';
import { NavItemComponent } from './nav-item/nav-item.component';
import { IframeComponent } from './iframe/iframe.component';
import {
    StorageBrowserComponent
} from './storage-browser/storage-browser.component';
import { NavCardComponent } from './nav-card/nav-card.component';
import { HighlightDirective } from './highlight.directive';
import {
    CategoryTreeViewComponent
} from './category-tree-view/category-tree-view.component';
import {
    CategoryTreeViewItemComponent
} from './category-tree-view-item/category-tree-view-item.component';
import { TagsEditorComponent } from './tags-editor/tags-editor.component';
import { RolesEditorComponent } from './roles-editor/roles-editor.component';
import {
    CategoryEditorComponent
} from './category-editor/category-editor.component';

@NgModule({
    declarations: [
        NavTopBarComponent,
        ConfirmComponent,
        NavSidebarComponent,
        NavItemComponent,
        IframeComponent,
        StorageBrowserComponent,
        NavCardComponent,
        HighlightDirective,
        CategoryTreeViewComponent,
        CategoryTreeViewItemComponent,
        TagsEditorComponent,
        RolesEditorComponent,
        CategoryEditorComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        LayoutModule,
        ScrollingModule,
        NgbAlertModule,
        NgbDropdownModule,
        NgbModalModule,
        NgbTooltipModule,
        NgbCollapseModule,
        NgbNavModule,
        NgbDatepickerModule,
        AppSharedModule,
    ],
    exports: [
        NavTopBarComponent,
        NavSidebarComponent,
        IframeComponent,
        NavCardComponent,
        HighlightDirective,
        CategoryTreeViewComponent,
        TagsEditorComponent,
        RolesEditorComponent,
        CategoryEditorComponent
    ]
})
export class AppCommonModule { }
