import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { ChartingComponent } from './charting/charting.component';


@NgModule({
    declarations: [
        SvgIconComponent,
        ChartingComponent
    ],
    imports: [
        HttpClientModule
    ],
    exports: [
        SvgIconComponent,
        ChartingComponent
    ]
})
export class AppSharedModule { }
