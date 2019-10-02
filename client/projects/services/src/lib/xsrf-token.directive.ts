import { Directive, OnInit, Inject } from '@angular/core';
import { XsrfService } from './xsrf.service';

@Directive({
    // tslint:disable-next-line: directive-selector
    selector: '[xsrf-token]'
})
export class XsrfTokenDirective implements OnInit {

    constructor(
        private xsrf: XsrfService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.xsrf.refresh();
    }

}
