import {
    AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild
} from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { TileMapService, TileMapModel } from '../tilemaps.service';
import { ArcGisService } from '../../arcgis.service';

@Component({
    selector: 'app-tilemaps-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.scss']
})
export class PreviewComponent implements AfterViewInit, OnInit, OnDestroy {

    private mapview?: __esri.MapView;

    public id = '';
    public name = '';

    @ViewChild('mapPreView', { static: false })
    public mapElRef!: ElementRef<HTMLDivElement>;

    constructor(
        private arcgis: ArcGisService,
        public activeModal: NgbActiveModal,
        public vm: TileMapService
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

    public ngOnInit(): void {
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

}
