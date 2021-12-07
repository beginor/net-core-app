import { DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, Input, ViewChild } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

import { DataApiParameterModel as ParameterModel } from '../dataapis.service';

@Component({
    selector: 'app-dataapi-debug',
    templateUrl: './debug.component.html',
    styleUrls: ['./debug.component.scss']
})
export class DebugComponent {

    @Input()
    public parameters: ParameterModel[] = [];
    @Input()
    public id: string = '';

    @ViewChild('statementEditor', { static: false })
    public statementEditorRef!: ElementRef<HTMLIFrameElement>;

    public codeEditorUrl: SafeUrl;

    constructor(
        @Inject('codeEditorUrl') codeEditorUrl: string,
        domSanitizer: DomSanitizer,
        @Inject(DOCUMENT) doc: Document
    ) {
        this.codeEditorUrl = domSanitizer.bypassSecurityTrustResourceUrl(
            codeEditorUrl
        );
    }

}
