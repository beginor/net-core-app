import {
    Component, ElementRef, NgZone, OnDestroy, OnInit, ViewChild
} from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Style, Layer } from 'mapbox-gl';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';

import {
    StorageBrowserComponent, UiService
} from 'projects/web/src/app/common';
import { VectortileService, VectortileModel } from '../vectortiles.service';

@Component({
    selector: 'app-vectortile-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.scss'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class DetailComponent implements OnInit, OnDestroy {

    public animation = '';
    public title = '';
    public editable = false;
    public model: VectortileModel = { id: '' };
    public styleFileName = '选择默认样式';
    @ViewChild('styleFile', { static: false })
    public styleFileRef?: ElementRef<HTMLInputElement>;

    private id = '';
    private reloadList = false;
    private styleChangeHandle = this.onStyleFileChange.bind(this);

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private modal: NgbModal,
        private zone: NgZone,
        private ui: UiService,
        public account: AccountService,
        public vm: VectortileService
    ) {
        const { id, editable } = route.snapshot.params;
        if (id === '0') {
            this.title = '新建矢量切片包';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑矢量切片包';
            this.editable = true;
        }
        else {
            this.title = '查看矢量切片包';
            this.editable = false;
        }
        this.id = id as string;
    }

    public async ngOnInit(): Promise<void> {
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                this.model = model;
                if (!!model.defaultStyle) {
                    this.styleFileName = model.defaultStyle;
                }
            }
        }
        this.styleFileRef?.nativeElement.addEventListener(
            'change',
            this.styleChangeHandle
        );
    }

    public ngOnDestroy(): void {
        this.styleFileRef?.nativeElement.removeEventListener(
            'change',
            this.styleChangeHandle
        );
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
        if (!!this.model.styleContent) {
            // fix layer source;
            const style = JSON.parse(this.model.styleContent) as Style;
            const assetsUrl = this.vm.getVectorTileAssetsUrl();
            style.sprite = `${assetsUrl}sprite`;
            style.glyphs = `${assetsUrl}glyphs/{fontstack}/{range}.pbf`;
            const layerUrl = this.vm.getVectorTileLayerUrl(
                this.model.id ?? '{id}'
            );
            style.sources = {
                mapbox: {
                    type: 'vector',
                    tiles: [`${layerUrl}/tile/{z}/{y}/{x}`]
                }
            };
            style.layers?.forEach((layer: Layer) => {
                layer.source = 'mapbox';
            });
            // add extent to style metadata;
            if (!!this.model.minLatitude && !!this.model.minLongitude
                && !!this.model.maxLatitude && !!this.model.maxLongitude
            ) {
                const metadata = style.metadata || {};
                metadata['mapbox:extent'] = [
                    this.model.minLongitude,
                    this.model.minLatitude,
                    this.model.maxLongitude,
                    this.model.maxLatitude
                ];
                style.metadata = metadata;
            }
            this.model.styleContent = JSON.stringify(style);
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
            this.model.directory = `${params.alias}:${path}`;
        })
        .catch(_ => { /* ignore error. */ });
    }

    private async onStyleFileChange(e: Event): Promise<void> {
        const files = this.styleFileRef?.nativeElement.files;
        if (!!files) {
            const file = files[0];
            this.zone.run(() => {
                this.styleFileName = file.name;
            });
            this.model.defaultStyle = file.name;
            try {
                const text = await file.text();
                this.model.styleContent = text;
            }
            catch (ex) {
                this.ui.showAlert(
                    { type: 'danger', message: '无法识别默认样式， 请重新选择！' }
                );
                console.error(ex);
            }
        }
    }

}
