import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import {
    Component, ElementRef, Inject, Input, OnInit, ViewChild
} from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { lastValueFrom } from 'rxjs';

import { AccountService } from 'app-shared';

import {
    DataApiService,
    DataApiParameterModel as ParameterModel,
    ResultType
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
    public resultType: ResultType = 'data';
    public paramNames: { [key: string]: boolean } = { };
    public paramValues: { [key: string]: string } = { };
    public loading = false;

    constructor(
        public vm: DataApiService,
        private http: HttpClient,
        private account: AccountService,
        @Inject(DOCUMENT) private doc: Document,
        @Inject('codeEditorUrl') codeEditorUrl: string,
        domSanitizer: DomSanitizer
    ) {
        this.codeEditorUrl = domSanitizer.bypassSecurityTrustResourceUrl(
            codeEditorUrl
        );
    }

    public async ngOnInit(): Promise<void> {
        if (this.parameters.length > 0) {
            return;
        }
        const model = await this.vm.getById(this.id);
        if (!model) {
            return;
        }
        this.parameters = model.parameters || [];
    }

    public initPreview(): void {
        const editorWin = this.previewFrameRef.nativeElement.contentWindow;
        editorWin?.postMessage(
            { language: 'txt', value: '请设置参数并调用！' },
            '*'
        );
    }

    public getApiPrefix(): string {
        const log = this.doc.location;
        return log.protocol + '//' + this.doc.location.host
    }

    public getApiUrl(): string {
        const params = new URLSearchParams();
        let paramCount = 0;
        Object.keys(this.paramNames).forEach(key => {
            if (!!this.paramNames[key] && !!this.paramValues[key]) {
                params.set(key, this.paramValues[key]);
                paramCount++;
            }
        });
        let url = this.vm.getApiUrl(this.id, this.resultType);
        if (paramCount > 0) {
            url += '?' + params.toString();
        }
        return url;
    }

    public async sendRequest(): Promise<void> {
        this.loading = true;
        const url = this.getApiUrl();
        const editorWin = this.previewFrameRef.nativeElement.contentWindow;
        const headers = {};
        this.account.addAuthTokenTo(headers);
        let value = await lastValueFrom(
            this.http.get(
                url,
                {
                    responseType: 'text',
                    headers
                }
            )
        );
        if (this.resultType != 'sql') {
            value = JSON.stringify(JSON.parse(value));
        }
        editorWin?.postMessage(
            { language: this.resultType === 'sql' ? 'sql' : 'json', value },
            '*'
        );
        this.loading = false;
    }

}
