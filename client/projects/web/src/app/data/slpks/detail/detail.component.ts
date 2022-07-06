import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import {
    StorageBrowserComponent
} from 'projects/web/src/app/common';
import { slideInRight, slideOutRight, AccountService } from 'app-shared';
import { SlpkService, SlpkModel } from '../slpks.service';

@Component({
    selector: 'app-slpk-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.css'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class DetailComponent implements OnInit {

    public animation = '';
    public title = '';
    public editable = false;
    public model: SlpkModel = { id: '', tags: [], roles: [], category: {} };
    public newTag = '';

    private id = '';
    private reloadList = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private modal: NgbModal,
        public account: AccountService,
        public vm: SlpkService
    ) {
        const { id, editable } = route.snapshot.params;
        if (id === '0') {
            this.title = '新建slpk 航拍模型';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑slpk 航拍模型';
            this.editable = true;
        }
        else {
            this.title = '查看slpk 航拍模型';
            this.editable = false;
        }
        this.id = id as string;
    }

    public async ngOnInit(): Promise<void> {
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                if (!model.roles) {
                    model.roles = [];
                }
                if (!model.tags) {
                    model.tags = [];
                }
                if (!model.category) {
                    model.category = {};
                }
                this.model = model;
            }
        }
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.fromState === '' && e.toState === 'void') {
            await this.router.navigate(['../'], { relativeTo: this.route });
            if (this.reloadList) {
                void this.vm.search();
            }
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

    public async save(): Promise<void> {
        if (typeof this.model.tags === 'string') {
            const tags = this.model.tags as string;
            this.model.tags = tags.split(',');
        }
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.reloadList = true;
        this.goBack();
    }

    public showFolderModal(): void {
        const modalRef = this.modal.open(
            StorageBrowserComponent,
            { size: 'xl', backdrop: 'static', keyboard: false }
        );
        const params = {
            alias: 'gisdata',
            path: '.',
            filter: '*.*'
        };
        Object.assign(modalRef.componentInstance, {
            title: '选择模型目录',
            params
        });
        modalRef.result.then((path: string) => {
            this.model.directory = `${params.alias}:${path}`;
        })
        .catch(_ => { /* ignore error. */ });
    }

}
