import {
    AfterViewInit, Component, ElementRef, OnDestroy, ViewChild
} from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { TileMapService } from '../tilemaps.service';
import { ArcGisService } from '../../arcgis.service';

@Component({
    selector: 'app-tilemaps-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.css']
})
export class PreviewComponent implements AfterViewInit, OnDestroy {

    private mapview?: __esri.MapView;

    public id = '';
    public name = '';

    @ViewChild('mapPreView', { static: true })
    public mapElRef!: ElementRef<HTMLDivElement>;

    constructor(
        public activeModal: NgbActiveModal,
        public vm: TileMapService,
        private arcgis: ArcGisService
    ) { }

    public ngAfterViewInit(): void {
        if (!this.mapElRef) {
            return;
        }
        const url = this.getTileLayerUrl();
        if (!url) {
            return;
        }
        this.arcgis.createTileLayerPreview(
            this.mapElRef.nativeElement, url
        ).then(mapview => this.mapview = mapview);
    }

    public ngOnDestroy(): void {
        if (!!this.mapview) {
            this.mapview.destroy();
        }
    }

    public getTileLayerUrl(): string {
        if (!this.id) {
            return '';
        }
        return this.vm.getTileLayerUrl(this.id);
    }

    public getTileUrlFormat(): string {
        const layerUrl = this.getTileLayerUrl();
        if (!layerUrl) {
            return '';
        }
        return layerUrl + '/tiles/{z}/{y}/{x}'
    }

}
