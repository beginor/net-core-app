import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
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
import { AccountComponent } from './account/account.component';
import { UserInfoComponent } from './user-info/user-info.component';
import {
    UserTokenListComponent
} from './user-token-list/user-token-list.component';
import {
    UserTokenDetailComponent
} from './user-token-detail/user-token-detail.component';

@NgModule({
    declarations: [
        NavTopBarComponent,
        ConfirmComponent,
        NavSidebarComponent,
        NavItemComponent,
        IframeComponent,
        StorageBrowserComponent,
        NavCardComponent,
        AccountComponent,
        UserInfoComponent,
        UserTokenListComponent,
        UserTokenDetailComponent
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
        NgbNavModule,
        NgbDatepickerModule,
        AppSharedModule
    ],
    exports: [
        NavTopBarComponent,
        NavSidebarComponent,
        IframeComponent,
        NavCardComponent
    ],
    entryComponents: [
        ConfirmComponent
    ]
})
export class AppCommonModule { }
