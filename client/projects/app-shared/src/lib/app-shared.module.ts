import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { EchartComponent } from './echart/echart.component';
import { FileSizePipe } from './file-size.pipe';
import { TrimInputDirective } from './trim-input-directive';

@NgModule({
    declarations: [
        SvgIconComponent,
        EchartComponent,
        FileSizePipe,
        TrimInputDirective,
    ],
    imports: [
        HttpClientModule,
        FormsModule,
    ],
    exports: [
        SvgIconComponent,
        EchartComponent,
        FileSizePipe,
        TrimInputDirective,
    ]
})
export class AppSharedModule { }
