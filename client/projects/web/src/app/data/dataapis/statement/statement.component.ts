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

    public updating = false;
    public updatingColumns = false;
    public testing = false;

    private id: string;
    private win: Window;
    private receiveMessageHandler = this.onReceiveMessage.bind(this);
    private statementLoadedToEditor = false;

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
        await this.loadData();
        this.win.addEventListener('message', this.receiveMessageHandler);
    }

    public ngOnDestroy(): void {
        this.win.removeEventListener('message', this.receiveMessageHandler);
    }

    public async loadData(): Promise<void> {
        this.updating = true;
        const model = await this.vm.getById(this.id);
        if (!!model) {
            this.model = model;
            this.title = `编辑 ${model.name} 指令`;
            if (!this.statementLoadedToEditor) {
                this.loadStatement();
            }
        }
        this.updating = false;
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
        editorWin.postMessage('addSmartSqlSupport', '*');
        editorWin.postMessage({
            language: 'xml',
            value: this.model.statement
        }, '*');
        this.statementLoadedToEditor = true;
    }

    public async getColumns(): Promise<void> {
        this.updatingColumns = true;
        const columns = await this.vm.getColumns(this.id);
        if (!!columns) {
            var oc = this.model.columns;
            columns.forEach(col => {
                const desc = oc?.find(c => c.name == col.name)?.description;
                if (!!desc && !col.description) {
                    col.description = desc;
                }
            });
            this.model.columns = columns;
        }
        this.updatingColumns = false;
    }

    public canDoTest(): boolean {
        const stmt = this.model.statement;
        if (!stmt) {
            return false;
        }
        const params = this.model.statement;
        if (!params || params.length === 0) {
            return false;
        }
        const cols = this.model.columns;
        if (!cols || cols.length === 0) {
            return false;
        }
        return true;
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
