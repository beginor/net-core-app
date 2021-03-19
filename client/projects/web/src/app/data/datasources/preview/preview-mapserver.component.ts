import {
    Component, ElementRef, Input, OnDestroy, AfterViewInit, ViewChild,
    Output, EventEmitter
} from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { loadModules } from 'esri-loader';

import { DataSourceModel, DataSourceService } from '../datasources.service';
import { ArcGisService } from '../../arcgis.service';

@Component({
    selector: 'app-datasources-preview-mapserver',
    template: `<div #mapEl class="mapview"></div>`,
    styles: [
        ':host { display: flex; height: 100%; }',
        'div.mapview { flex: 1; }'
    ]
})
export class PreviewMapServerComponent implements AfterViewInit, OnDestroy {
    @Input()
    public ds: DataSourceModel = { id: '' };
    @Output() public downloadProgress = new EventEmitter<number>(true);
    @ViewChild('mapEl')
    public mapElRef!: ElementRef<HTMLDivElement>;

    private mapview?: __esri.MapView;

    constructor(
        private http: HttpClient,
        private vm: DataSourceService,
        private arcgis: ArcGisService
    ) { }

    public async ngAfterViewInit(): Promise<void> {
        this.mapview = await this.arcgis.createMapView(
            this.mapElRef.nativeElement
        );
        await this.mapview.when();
        await this.loadFeatureSet();
    }

    public ngOnDestroy(): void {
        if (!!this.mapview) {
            this.mapview.destroy();
        }
    }

    private async loadFeatureSet(): Promise<void> {
        const id = this.ds.id;
        if (!id) {
            return;
        }
        const [FeatureLayer] = await loadModules<[
            __esri.FeatureLayerConstructor
        ]>([
            'esri/layers/FeatureLayer'
        ]);
        const featureLayer = new FeatureLayer({
            url: this.vm.getPreviewUrl(id, 'mapserver')
        });
        this.mapview?.map.add(featureLayer);
    }

}
