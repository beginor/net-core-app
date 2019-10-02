import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { XsrfTokenDirective } from './xsrf-token.directive';


@NgModule({
    declarations: [
        XsrfTokenDirective
    ],
    imports: [
        HttpClientModule
    ],
    exports: [
        XsrfTokenDirective
    ]
})
export class ServicesModule { }
