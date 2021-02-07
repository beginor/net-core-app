import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DataRoutingModule } from './data-routing.module';
import { DashbordComponent } from './dashbord/dashbord.component';

@NgModule({
    declarations: [DashbordComponent],
    imports: [
        CommonModule,
        DataRoutingModule
    ]
})
export class DataModule { }
