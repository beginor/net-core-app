import {
    AfterViewInit, Component, ElementRef, OnDestroy, ViewChild
} from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { SlpkService } from '../slpks.service';
import { ArcGisService } from '../../arcgis.service';

@Component({
    selector: 'app-slpks-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.scss']
})
export class PreviewComponent implements AfterViewInit, OnDestroy {

    public id = '';
    public name = '';

    @ViewChild('mapPreView', { static: true })
    public mapElRef!: ElementRef<HTMLDivElement>;

    private mapview?: __esri.SceneView;

    constructor(
        public activeModal: NgbActiveModal,
        public vm: SlpkService,
        private arcgis: ArcGisService
    ) { }

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
        ).then(mapview => this.mapview = mapview);
    }

    public ngOnDestroy(): void {
        if (!!this.mapview) {
            this.mapview.destroy();
        }
    }

    public getSlpkLayerUrl(): string {
        if (!this.id) {
            return '';
        }
        return this.vm.getSlpkLayerUrl(this.id);
    }

}
