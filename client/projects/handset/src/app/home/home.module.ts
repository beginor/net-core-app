import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatModule } from '../mat/mat.module';

import { HomeRoutingModule } from './home-routing.module';
import { HomeComponent } from './home/home.component';

@NgModule({
    declarations: [HomeComponent],
    imports: [
        CommonModule,
        HomeRoutingModule,
        MatModule
    ]
})
export class HomeModule { }
