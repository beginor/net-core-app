import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

import { PrivilegesRoutingModule } from './privileges-routing.module';
import { ListComponent } from './list/list.component';
import { AppCommonModule } from '../../common';


@NgModule({
    declarations: [
        ListComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        NgbPaginationModule,
        AppCommonModule,
        PrivilegesRoutingModule
    ]
})
export class PrivilegesModule { }
