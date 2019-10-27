import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
    NgbPaginationModule, NgbTooltipModule
} from '@ng-bootstrap/ng-bootstrap';

import { ServicesModule } from 'services';

import { AppCommonModule } from 'projects/web/src/app/common';
import { RolesRoutingModule } from './roles-routing.module';
import { ListComponent } from './list/list.component';
import { DetailComponent } from './detail/detail.component';
import { PrivilegeComponent } from './privilege/privilege.component';
import { UserComponent } from './user/user.component';

@NgModule({
    declarations: [
        ListComponent,
        DetailComponent,
        PrivilegeComponent,
        UserComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        NgbPaginationModule,
        NgbTooltipModule,
        ServicesModule,
        AppCommonModule,
        RolesRoutingModule
    ]
})
export class RolesModule { }
