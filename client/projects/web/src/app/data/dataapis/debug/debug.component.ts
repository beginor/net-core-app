import { DOCUMENT } from '@angular/common';
import {
    Component, ElementRef, Inject, Input, OnInit, ViewChild
} from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

import {
    DataApiService,
    DataApiParameterModel as ParameterModel
} from '../dataapis.service';

@Component({
    selector: 'app-dataapi-debug',
    templateUrl: './debug.component.html',
    styleUrls: ['./debug.component.scss']
})
export class DebugComponent implements OnInit {

    @Input()
    public parameters: ParameterModel[] = [];
    @Input()
    public id: string = '';

    @ViewChild('previewFrame', { static: true })
    public previewFrameRef!: ElementRef<HTMLIFrameElement>;

    public codeEditorUrl: SafeUrl;

    constructor(
        private vm: DataApiService,
        @Inject('codeEditorUrl') codeEditorUrl: string,
        domSanitizer: DomSanitizer,
        @Inject(DOCUMENT) doc: Document
    ) {
        this.codeEditorUrl = domSanitizer.bypassSecurityTrustResourceUrl(
            codeEditorUrl
        );
    }

    public async ngOnInit(): Promise<void> {
        console.info(JSON.stringify(this.parameters));
    }

    public initPreview(): void {
        const editorWin = this.previewFrameRef.nativeElement.contentWindow;
        editorWin?.postMessage(
            { language: 'txt', value: '请设置参数并调用！' },
            '*'
        );
    }

}
