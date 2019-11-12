import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
    NgbPaginationModule, NgbTooltipModule
} from '@ng-bootstrap/ng-bootstrap';

import { ServicesModule } from 'services';

import { AppCommonModule } from '../../common';
import { UsersRoutingModule } from './users-routing.module';
import { ListComponent } from './list/list.component';
import { DetailComponent } from './detail/detail.component';


@NgModule({
  declarations: [
      ListComponent,
      DetailComponent
    ],
  imports: [
    CommonModule,
    FormsModule,
    NgbPaginationModule,
    NgbTooltipModule,
    AppCommonModule,
    UsersRoutingModule
  ]
})
export class UsersModule { }
