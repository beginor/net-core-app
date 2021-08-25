import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';

import {
    ServerFolderService, ServerFolderModel
} from '../server-folders.service';

@Component({
    selector: 'app-server-folder-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.scss'],
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
    public model: ServerFolderModel = { id: '', aliasName: '', rootFolder: '', readonly: false }; // eslint-disable-line max-len

    private id = '';
    private reloadList = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: ServerFolderService
    ) {
        const { id, editable } = route.snapshot.params;
        if (id === '0') {
            this.title = '新建服务器目录';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑服务器目录';
            this.editable = true;
        }
        else {
            this.title = '查看服务器目录';
            this.editable = false;
        }
        this.id = id as string;
    }

    public async ngOnInit(): Promise<void> {
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
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
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.reloadList = true;
        this.goBack();
    }

}
