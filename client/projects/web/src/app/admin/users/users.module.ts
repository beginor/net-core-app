import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
    NgbPaginationModule, NgbTooltipModule, NgbDatepickerModule,
    NgbDropdownModule
} from '@ng-bootstrap/ng-bootstrap';

import { AppSharedModule } from 'app-shared';

import { AppCommonModule } from '../../common';
import { UsersRoutingModule } from './users-routing.module';
import { ListComponent } from './list/list.component';
import { DetailComponent } from './detail/detail.component';
import { PasswordComponent } from './password/password.component';
import { LockComponent } from './lock/lock.component';
import { RolesComponent } from './roles/roles.component';

@NgModule({
  declarations: [
      ListComponent,
      DetailComponent,
      PasswordComponent,
      LockComponent,
      RolesComponent
    ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbPaginationModule,
    NgbTooltipModule,
    NgbDatepickerModule,
    NgbDropdownModule,
    AppCommonModule,
    AppSharedModule,
    UsersRoutingModule
  ]
})
export class UsersModule { }
