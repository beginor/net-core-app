import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { EchartComponent } from './echart/echart.component';


@NgModule({
    declarations: [
        SvgIconComponent,
        EchartComponent,
    ],
    imports: [
        HttpClientModule,
    ],
    exports: [
        SvgIconComponent,
        EchartComponent,
    ]
})
export class AppSharedModule { }
