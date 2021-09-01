import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ScrollingModule } from '@angular/cdk/scrolling';
import {
    NgbAlertModule, NgbDropdownModule, NgbModalModule, NgbTooltipModule,
    NgbCollapseModule
} from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { NavTopBarComponent } from './nav-topbar/nav-topbar.component';
import { ConfirmComponent } from './confirm/confirm.component';
import { NavSidebarComponent } from './nav-sidebar/nav-sidebar.component';
import { NavItemComponent } from './nav-item/nav-item.component';
import { IframeComponent } from './iframe/iframe.component';
import {
    ServerFolderBrowserComponent
} from './storage/storage.component';

@NgModule({
    declarations: [
        NavTopBarComponent,
        ConfirmComponent,
        NavSidebarComponent,
        NavItemComponent,
        IframeComponent,
        ServerFolderBrowserComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        ScrollingModule,
        NgbAlertModule,
        NgbDropdownModule,
        NgbModalModule,
        NgbTooltipModule,
        NgbCollapseModule,
        AppSharedModule
    ],
    exports: [
        NavTopBarComponent,
        NavSidebarComponent,
        IframeComponent
    ],
    entryComponents: [
        ConfirmComponent
    ]
})
export class AppCommonModule { }
