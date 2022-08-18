import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';
import { TileMapService, TileMapModel } from '../tilemaps.service';
import { StorageBrowserComponent } from '../../../common';

@Component({
    selector: 'app-tilemap-detail',
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
    public model: TileMapModel = { id: '', tags: [], roles: [], category: {} };

    private id = '';
    private reloadList = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private modal: NgbModal,
        public account: AccountService,
        public vm: TileMapService
    ) {
        const { id, editable } = route.snapshot.params;
        if (id === '0') {
            this.title = '新建栅格切片包';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑栅格切片包';
            this.editable = true;
        }
        else {
            this.title = '查看栅格切片包';
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
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.reloadList = true;
        this.goBack();
    }

    public showCacheFolderModal(): void {
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
            title: '选择缓存目录',
            params
        });
        modalRef.result.then((path: string) => {
            this.model.cacheDirectory = `${params.alias}:${path}`;
        })
        .catch(_ => { /* ignore error. */ });
    }

    public showFolderDialog(): void {
        const modalRef = this.modal.open(
            StorageBrowserComponent,
            { size: 'xl', backdrop: 'static', keyboard: false }
        );
        const params = {
            alias: 'gisdata',
            path: '.',
            filter: '*.json'
        };
        Object.assign(modalRef.componentInstance, {
            title: '选择切片信息',
            params
        });
        modalRef.result.then((path: string) => {
            this.model.mapTileInfoPath = `${params.alias}:${path}`;
        }).catch(_ => { /* ignore select error */ });
    }

    public onFolderStructureChange(): void {
        this.model.isBundled = this.model.folderStructure === 'esri';
    }

}
