import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatModule } from '../mat/mat.module';

import { AboutRoutingModule } from './about-routing.module';
import { AboutComponent } from './about/about.component';

@NgModule({
    declarations: [
        AboutComponent
    ],
    imports: [
        CommonModule,
        AboutRoutingModule,
        MatModule
    ]
})
export class AboutModule { }
