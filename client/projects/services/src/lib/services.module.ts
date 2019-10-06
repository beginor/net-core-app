import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { AntiforgeryDirective } from './antiforgery.directive';


@NgModule({
    declarations: [
        AntiforgeryDirective
    ],
    imports: [
        HttpClientModule
    ],
    exports: [
        AntiforgeryDirective
    ]
})
export class ServicesModule { }
