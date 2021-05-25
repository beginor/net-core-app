import { Component } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';

import { NavigationService } from '../services/navigation.service';

@Component({
    selector: 'app-iframe',
    templateUrl: './iframe.component.html',
    styleUrls: ['./iframe.component.scss']
})
export class IframeComponent {

    public safeUrl?: SafeUrl;

    constructor(
        actRoute: ActivatedRoute,
        domSanitizer: DomSanitizer,
        nav: NavigationService
    ) {
        actRoute.params.subscribe(param => {
            const src: string = param['src'];
            if (src.startsWith('http://') || src.startsWith('https://')) {
                this.safeUrl = domSanitizer.bypassSecurityTrustResourceUrl(src);
            }
            else {
                const iframeUrl = nav.findCurrentIframeUrl();
                if (!!iframeUrl) {
                    this.safeUrl = domSanitizer.bypassSecurityTrustResourceUrl(
                        iframeUrl
                    );
                }
            }
        });
    }

}
