import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { AppCommonModule } from '../common/app-common.module';
import { AccountRoutingModule } from './account-routing.module';
import { UserInfoComponent } from './user-info/user-info.component';
import { TokenListComponent } from './token-list/token-list.component';
import { TokenDetailComponent } from './token-detail/token-detail.component';


@NgModule({
    declarations: [
        UserInfoComponent,
        TokenListComponent,
        TokenDetailComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NgbDatepickerModule,
        AppCommonModule,
        AppSharedModule,
        AccountRoutingModule
    ]
})
export class AccountModule { }
