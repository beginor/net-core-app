import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import {
    Component, ElementRef, ErrorHandler, Inject, Input, OnInit, ViewChild
} from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { lastValueFrom } from 'rxjs';
import { Map as MapboxMap } from 'mapbox-gl';

import { AccountService } from 'app-shared';
import { UiService } from '../../../common'
import { OptionsService } from "../../options.service";

import {
    DataApiService,
    DataApiParameterModel as ParameterModel,
    ResultType
} from '../dataapis.service';
import { PreviewGeoJsonComponent } from '../preview/preview-geojson.component';

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
    @Input()
    public hasGeoColumn = false;

    @ViewChild('previewFrame', { static: false })
    public previewFrameRef?: ElementRef<HTMLIFrameElement>;
    @ViewChild(PreviewGeoJsonComponent, { static: false })
    public previewMapRef?: PreviewGeoJsonComponent;

    public codeEditorUrl: SafeUrl;
    public resultType: ResultType = 'data';
    public paramNames: { [key: string]: boolean } = { };
    public paramValues: { [key: string]: string } = { };
    public loading = false;

    constructor(
        public vm: DataApiService,
        public account: AccountService,
        private http: HttpClient,
        private ui: UiService,
        private errorHandler: ErrorHandler,
        @Inject(DOCUMENT) private doc: Document,
        domSanitizer: DomSanitizer,
        options: OptionsService
    ) {
        this.codeEditorUrl = domSanitizer.bypassSecurityTrustResourceUrl('');
        void options.loadEditorOptions().then(editor => {
            this.codeEditorUrl = domSanitizer.bypassSecurityTrustResourceUrl(
                editor.url
            );
        }).catch(ex => {
            ui.showAlert({ type: 'danger', message: '加载编辑器配置出错！' });
            console.error(ex);
        });
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
        const editorWin = this.previewFrameRef?.nativeElement.contentWindow;
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
        const headers = {};
        this.account.addAuthTokenTo(headers);
        try {
            let value = await lastValueFrom(
                this.http.get(
                    url,
                    {
                        responseType: 'text',
                        headers
                    }
                )
            );
            if (this.resultType === 'geojson') {
                this.previewMapRef?.setData(JSON.parse(value));
                return;
            }
            if (this.resultType !== 'sql') {
                value = JSON.stringify(JSON.parse(value));
            }
            const prvFrame = this.previewFrameRef?.nativeElement.contentWindow;
            prvFrame?.postMessage(
                { language: this.resultType === 'sql' ? 'sql' : 'json', value },
                '*'
            );

        }
        catch (ex: unknown) {
            this.ui.showAlert(
                { type: 'danger', 'message': '调用 API 出错！' }
            );
            this.errorHandler.handleError(ex);
        }
        finally {
            this.loading = false;
        }
    }

    public onMapLoaded(map: MapboxMap): void {

    }

}
