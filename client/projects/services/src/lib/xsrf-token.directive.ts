import { Directive, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { XsrfGuard } from 'services/services';

@Directive({
    // tslint:disable-next-line: directive-selector
    selector: '[xsrf-token]'
})
export class XsrfTokenDirective implements OnInit {

    constructor(
        private xsrf: XsrfGuard
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.xsrf.refresh();
    }

}
