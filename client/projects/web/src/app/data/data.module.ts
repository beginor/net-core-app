import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AppCommonModule } from '../common';

import { DataRoutingModule } from './data-routing.module';
import { DashbordComponent } from './dashbord/dashbord.component';

@NgModule({
    declarations: [DashbordComponent],
    imports: [
        CommonModule,
        AppCommonModule,
        DataRoutingModule
    ]
})
export class DataModule { }
