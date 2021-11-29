import { Component, Inject, OnInit, ElementRef, ViewChild } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';

import { DataApiService, DataApiModel } from '../dataapis.service';

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
export class StatementComponent implements OnInit {

    public animation = '';
    public title = '';
    public model: DataApiModel = { id: '' };
    public codeEditorUrl: SafeUrl;

    @ViewChild('statementEditor', { static: false })
    public statementEditorRef!: ElementRef<HTMLIFrameElement>;
    @ViewChild('#statementEditor', { static: false })
    public previewEditorRef!: ElementRef<HTMLIFrameElement>;

    private id: string;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: DataApiService,
        @Inject('codeEditorUrl') codeEditorUrl: string,
        domSanitizer: DomSanitizer
    ) {
        const { id } = route.snapshot.params;
        this.id = id;
        this.codeEditorUrl = domSanitizer.bypassSecurityTrustResourceUrl(codeEditorUrl);
    }

    public async ngOnInit(): Promise<void> {
        const model = await this.vm.getById(this.id);
        if (!!model) {
            this.model = model;
            this.title = `编辑 ${model.name} 指令`;
        }
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
        //
    }

    public loadStatement(): void {
        if (!this.statementEditorRef) {
            return;
        }
        const editorWindow = this.statementEditorRef.nativeElement.contentWindow;
        if (!editorWindow) {
            return;
        }
        if (!this.model.statement) {
            return;
        }
        editorWindow.postMessage({
            language: 'xml',
            value: this.model.statement
        }, '*');
    }

}
