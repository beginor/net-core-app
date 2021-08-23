import {
    AfterViewInit, Component, ElementRef, OnDestroy, ViewChild
} from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { ArcGisService } from '../../arcgis.service';
import { VectortileService } from '../vectortiles.service';

@Component({
    selector: 'app-vectortiles-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.scss']
})
export class PreviewComponent implements AfterViewInit, OnDestroy {

    private mapview?: __esri.MapView;

    public id = '';
    public name = '';

    @ViewChild('mapPreView', { static: true })
    public mapElRef!: ElementRef<HTMLDivElement>;

    constructor(
        public activeModal: NgbActiveModal,
        public vm: VectortileService,
        private arcgis: ArcGisService
    ) { }

    public ngAfterViewInit(): void {
        if (!this.mapElRef) {
            return;
        }
        const url = this.getVectorTileLayerUrl();
        if (!url) {
            return;
        }
        void this.arcgis.createVectorTileLayerPreview(
            this.mapElRef.nativeElement,
            this.vm.getVectorTileLayerStyleUrl(this.id)
        ).then(mapview => this.mapview = mapview);
    }

    public ngOnDestroy(): void {
        if (!!this.mapview) {
            this.mapview.destroy();
        }
    }

    public getVectorTileLayerUrl(): string {
        if (!this.id) {
            return '';
        }
        return this.vm.getVectorTileLayerUrl(this.id);
    }

}
