import { Component } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'lib-iframe',
    templateUrl: './iframe.component.html',
    styleUrls: ['./iframe.component.scss']
})
export class IframeComponent {

    public src?: SafeUrl;

    constructor(
        actRoute: ActivatedRoute,
        domSanitizer: DomSanitizer
    ) {
        actRoute.params.subscribe(param => {
            const src = param['src'];
            this.src = domSanitizer.bypassSecurityTrustResourceUrl(src);
        });
    }

}
