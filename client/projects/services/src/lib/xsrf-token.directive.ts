import { Directive, OnInit, Inject, Input } from '@angular/core';
import { AntiforgeryService } from './xsrf.service';

@Directive({
    // tslint:disable-next-line: directive-selector
    selector: '[antiforgery]'
})
export class AntiforgeryDirective implements OnInit {

    @Input('antiforgery')
    public enabled: boolean;

    constructor(
        private xsrf: AntiforgeryService
    ) { }

    public ngOnInit(): void {
        if (!this.enabled) {
            return;
        }
        this.xsrf.refresh();
    }

}
