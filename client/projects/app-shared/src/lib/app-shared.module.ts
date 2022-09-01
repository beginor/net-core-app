import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { EchartComponent } from './echart/echart.component';
import { FileSizePipe } from './file-size.pipe';


@NgModule({
    declarations: [
        SvgIconComponent,
        EchartComponent,
        FileSizePipe,
    ],
    imports: [
        HttpClientModule,
    ],
    exports: [
        SvgIconComponent,
        EchartComponent,
        FileSizePipe,
    ]
})
export class AppSharedModule { }
