import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import {
    NgbAlertModule, NgbDropdownModule, NgbModalModule, NgbTooltipModule
} from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { NavTopBarComponent } from './nav-topbar/nav-topbar.component';
import { ConfirmComponent } from './confirm/confirm.component';
import { NavSidebarComponent } from './nav-sidebar/nav-sidebar.component';
import { NavItemComponent } from './nav-item/nav-item.component';
import { IframeComponent } from './iframe/iframe.component';

@NgModule({
    declarations: [
        NavTopBarComponent,
        ConfirmComponent,
        NavSidebarComponent,
        NavItemComponent,
        IframeComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        NgbAlertModule,
        NgbDropdownModule,
        NgbModalModule,
        NgbTooltipModule,
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
