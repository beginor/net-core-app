import {
    Component, Inject, OnInit, ElementRef, ViewChild, OnDestroy, NgZone
} from '@angular/core';
import { DOCUMENT } from '@angular/common';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';

import { UiService } from '../../../common';
import {
    DataApiService, DataApiModel, DataApiParameterModel
} from '../dataapis.service';

@Component({
    selector: 'app-statement',
    templateUrl: './statement.component.html',
    styleUrls: ['./statement.component.scss'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class StatementComponent implements OnInit, OnDestroy {

    public animation = '';
    public title = '';
    public model: DataApiModel = { id: '' };
    public codeEditorUrl: SafeUrl;

    @ViewChild('statementEditor', { static: false })
    public statementEditorRef!: ElementRef<HTMLIFrameElement>;
    @ViewChild('#statementEditor', { static: false })
    public previewEditorRef!: ElementRef<HTMLIFrameElement>;

    public newParam: DataApiParameterModel = { type: 'string' };
    public paramTypes = [
        'string', 'int', 'long', 'float', 'double', 'datetime', 'bool',
        'string[]', 'int[]', 'long[]', 'float[]', 'double[]', 'datetime[]',
        'bool[]'
    ];

    public updating = false;
    public paramEditIndex = -1;
    public showNewParamRow = false;
    public updatingColumns = false;
    public columnEditIndex = -1;

    private id: string;
    private win: Window;
    private receiveMessageHandler = this.onReceiveMessage.bind(this);

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private zone: NgZone,
        public account: AccountService,
        public vm: DataApiService,
        public ui: UiService,
        @Inject('codeEditorUrl') codeEditorUrl: string,
        domSanitizer: DomSanitizer,
        @Inject(DOCUMENT) doc: Document
    ) {
        const { id } = route.snapshot.params;
        this.id = id as string;
        this.codeEditorUrl = domSanitizer.bypassSecurityTrustResourceUrl(
            codeEditorUrl
        );
        this.win = doc.defaultView as Window;
    }

    public async ngOnInit(): Promise<void> {
        const model = await this.vm.getById(this.id);
        if (!!model) {
            this.model = model;
            this.title = `编辑 ${model.name} 指令`;
        }
        this.win.addEventListener('message', this.receiveMessageHandler);
    }

    public ngOnDestroy(): void {
        this.win.removeEventListener('message', this.receiveMessageHandler);
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.fromState === '' && e.toState === 'void') {
            await this.router.navigate(['../../'], { relativeTo: this.route });
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

    public async save(): Promise<void> {
        this.updating = true;
        this.getStatement();
    }

    public loadStatement(): void {
        if (!this.statementEditorRef) {
            return;
        }
        const editorWin = this.statementEditorRef.nativeElement.contentWindow;
        if (!editorWin) {
            return;
        }
        if (!this.model.statement) {
            return;
        }
        editorWin.postMessage({
            language: 'xml',
            value: this.model.statement
        }, '*');
    }

    public addParameter(): void {
        if (!this.newParam.name) {
            this.ui.showAlert({ type: 'danger', message: '请输入参数名称！' });
            return;
        }
        if (!this.model.parameters) {
            this.model.parameters = [];
        }
        this.model.parameters.push(this.newParam);
        this.newParam = { type: 'string' };
        this.showNewParamRow = false;
    }

    public removeParameter(idx: number): void {
        this.model.parameters?.splice(idx, 1);
    }

    public async getColumns(): Promise<void> {
        this.updatingColumns = true;
        const columns = await this.vm.getColumns(this.id);
        if (!!columns) {
            var oldColumns = this.model.columns;
            this.model.columns = columns;
        }
        this.updatingColumns = false;
    }

    private getStatement(): void {
        const editorWin = this.statementEditorRef.nativeElement.contentWindow;
        if (!editorWin) {
            return;
        }
        editorWin.postMessage('getValue', '*');
    }

    private onReceiveMessage(e: MessageEvent<string>): void {
        this.model.statement = e.data;
        this.vm.update(this.id, this.model).finally(() => {
            this.zone.run(() => this.updating = false);
        });
    }

}
