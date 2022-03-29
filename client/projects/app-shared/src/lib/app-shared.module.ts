import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { FileSizePipe } from './file-size.pipe';


@NgModule({
    declarations: [
        SvgIconComponent,
        FileSizePipe
    ],
    imports: [
        HttpClientModule
    ],
    exports: [
        SvgIconComponent,
        FileSizePipe
    ]
})
export class AppSharedModule { }
