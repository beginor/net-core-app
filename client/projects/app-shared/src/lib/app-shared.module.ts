import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { SvgIconDirective } from './svg-icon.directive';


@NgModule({
    declarations: [
        SvgIconDirective
    ],
    imports: [
        HttpClientModule
    ],
    exports: [
        SvgIconDirective
    ]
})
export class AppSharedModule { }
