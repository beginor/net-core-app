import {
    Component, ElementRef, Input, OnDestroy, AfterViewInit, ViewChild,
    Output, EventEmitter
} from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { loadModules } from 'esri-loader';

import { DataSourceModel, DataSourceService } from '../datasources.service';
import { ArcGisService } from '../../arcgis.service';

@Component({
    selector: 'app-datasources-preview-featureset',
    template: `<div #mapEl class="mapview"></div>`,
    styles: [
        ':host { display: flex; height: 100%; }',
        'div.mapview { flex: 1; }'
    ]
})
export class PreviewFeatureSetComponent implements AfterViewInit, OnDestroy {
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
        const count = await this.vm.getCount(id, { });
        const json: any = await this.vm.getFeatureSetJson(
            id,
            { $take: count, $returnExtent: true },
            (total, loaded) => {
                const percent = Number.parseFloat((loaded / total).toFixed(2));
                this.downloadProgress.next(percent);
            }
        );
        const [FeatureSet, FeatureLayer, Extent] = await loadModules<[
            __esri.FeatureSetConstructor,
            __esri.FeatureLayerConstructor,
            __esri.ExtentConstructor
        ]>([
            'esri/tasks/support/FeatureSet',
            'esri/layers/FeatureLayer',
            'esri/geometry/Extent'
        ]);
        const extent = new Extent(json.extent);
        delete json.extent;
        const featureset = FeatureSet.fromJSON(json);
        const featureLayer = new FeatureLayer({
            geometryType: featureset.geometryType as any,
            fields: featureset.fields,
            source: featureset.features,
            spatialReference: featureset.spatialReference,
            objectIdField: json.objectIdFieldName
        });
        this.mapview?.map.add(featureLayer);
        await this.mapview?.goTo(extent);
    }

}
