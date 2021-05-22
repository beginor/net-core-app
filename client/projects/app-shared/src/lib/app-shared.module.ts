import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { SvgIconComponent } from './svg-icon/svg-icon.component';
import { IframeComponent } from './iframe/iframe.component';


@NgModule({
    declarations: [
        SvgIconComponent,
        IframeComponent
    ],
    imports: [
        HttpClientModule
    ],
    exports: [
        SvgIconComponent,
        IframeComponent
    ]
})
export class AppSharedModule { }
