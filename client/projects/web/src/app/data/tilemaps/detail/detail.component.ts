import {
    Component, OnInit, OnDestroy, ViewChild, AfterViewInit, ElementRef
} from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';
import { TileMapService, TileMapModel } from '../tilemaps.service';
import { ArcGisService } from '../../arcgis.service';

@Component({
    selector: 'app-tile-map-detail',
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
    public model: TileMapModel = {};
    @ViewChild('mapPreView', { static: false })
    private mapElRef!: ElementRef<HTMLDivElement>;

    private id = '';
    private reloadList = false;
    private mapview?: __esri.MapView;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private arcgis: ArcGisService,
        public account: AccountService,
        public vm: TileMapService
    ) {
        const id = route.snapshot.params.id;
        const editable = route.snapshot.params.editable;
        if (id === '0') {
            this.title = '新建切片地图';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑切片地图';
            this.editable = true;
        }
        else {
            this.title = '查看切片地图';
            this.editable = false;
        }
        this.id = id;
    }

    public ngAfterViewInit(): void {
        if (!this.mapElRef) {
            return;
        }
        const url = this.getLayerUrl();
        if (!url) {
            return;
        }
        this.arcgis.loadJsApi().then(
            () => this.arcgis.createLayerPreview(
                this.mapElRef.nativeElement, url
            )
        ).then(mapview => this.mapview = mapview);
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
        if (!!this.mapview) {
            this.mapview.destroy();
        }
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.phaseName === 'done' && e.toState === 'void') {
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
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.reloadList = true;
        this.goBack();
    }

    public getLayerUrl(): string {
        if (!this.id) {
            return '';
        }
        return this.vm.getLayerUrl(this.id);
    }

}
