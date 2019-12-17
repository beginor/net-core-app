import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import {
    NgbAlertModule, NgbDropdownModule, NgbModalModule, NgbTooltipModule
} from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { NavBarComponent } from './nav-bar/nav-bar.component';
import { ConfirmComponent } from './confirm/confirm.component';
import { NavSidebarComponent } from './nav-sidebar/nav-sidebar.component';

@NgModule({
    declarations: [
        NavBarComponent,
        ConfirmComponent,
        NavSidebarComponent
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
        NavBarComponent,
        NavSidebarComponent
    ],
    entryComponents: [
        ConfirmComponent
    ]
})
export class AppCommonModule { }
