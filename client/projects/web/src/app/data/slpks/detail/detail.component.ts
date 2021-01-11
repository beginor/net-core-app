import {
    AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild
} from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';
import { SlpkService, SlpkModel } from '../slpks.service';
import { ArcGisService } from '../../arcgis.service';

@Component({
    selector: 'app-slpk-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.scss'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class DetailComponent implements AfterViewInit, OnInit, OnDestroy {

    public animation = '';
    public title = '';
    public editable = false;
    public model: SlpkModel = { tags: [] };
    public newTag = '';
    @ViewChild('mapPreView', { static: false })
    public mapElRef!: ElementRef<HTMLDivElement>;

    private id = '';
    private reloadList = false;
    private mappreview?: __esri.SceneView;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private arcgis: ArcGisService,
        public account: AccountService,
        public vm: SlpkService
    ) {
        const id = route.snapshot.params.id;
        const editable = route.snapshot.params.editable;
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
        this.id = id;
    }

    public ngAfterViewInit(): void {
        if (!this.mapElRef) {
            return;
        }
        const url = this.getSlpkLayerUrl();
        if (!url) {
            return;
        }
        this.arcgis.createSlpkLayerPreview(
            this.mapElRef.nativeElement, url
        ).then(view => this.mappreview = view);
    }

    public async ngOnInit(): Promise<void> {
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                this.model = model;
            }
        }
    }

    public ngOnDestroy(): void {
        if (!!this.mappreview) {
            this.mappreview.destroy();
        }
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.fromState === '' && e.toState === 'void') {
            await this.router.navigate(['../'], { relativeTo: this.route });
            if (this.reloadList) {
                this.vm.search();
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

    public addNewTag(): void {
        if (!this.model.tags) {
            this.model.tags = [];
        }
        const newTag = this.newTag.trim();
        if (!!newTag && this.model.tags.indexOf(newTag) < 0) {
            this.model.tags.push(newTag);
            this.newTag = '';
        }
    }

    public delTag(tag: string): void {
        if (!this.model.tags) {
            return;
        }
        const idx = this.model.tags.indexOf(tag);
        this.model.tags?.splice(idx, 1);
    }

    public getSlpkLayerUrl(): string {
        if (!this.id) {
            return '';
        }
        return this.vm.getSlpkLayerUrl(this.id);
    }

}
