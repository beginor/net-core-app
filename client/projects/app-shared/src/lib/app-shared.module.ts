import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { SvgIconComponent } from './svg-icon/svg-icon.component';


@NgModule({
    declarations: [
        SvgIconComponent,
    ],
    imports: [
        HttpClientModule,
    ],
    exports: [
        SvgIconComponent,
    ]
})
export class AppSharedModule { }
