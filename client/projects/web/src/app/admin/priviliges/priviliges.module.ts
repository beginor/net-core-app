import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PrivilegesRoutingModule } from './priviliges-routing.module';
import { ListComponent } from './list/list.component';


@NgModule({
  declarations: [ListComponent],
  imports: [
    CommonModule,
    PrivilegesRoutingModule
  ]
})
export class PrivilegesModule { }
