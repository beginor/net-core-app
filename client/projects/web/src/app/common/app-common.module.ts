import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import {
    NgbAlertModule, NgbDropdownModule, NgbModalModule
} from '@ng-bootstrap/ng-bootstrap';

import { ServicesModule } from 'services';

import { NavBarComponent } from './nav-bar/nav-bar.component';
import { ConfirmComponent } from './confirm/confirm.component';

@NgModule({
    declarations: [
        NavBarComponent,
        ConfirmComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        NgbAlertModule,
        NgbDropdownModule,
        NgbModalModule,
        ServicesModule
    ],
    exports: [
        NavBarComponent
    ],
    entryComponents: [
        ConfirmComponent
    ]
})
export class AppCommonModule { }
