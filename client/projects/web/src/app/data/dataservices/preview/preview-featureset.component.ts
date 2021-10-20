import {
    Component, ElementRef, Input, OnDestroy, AfterViewInit, ViewChild,
    Output, EventEmitter
} from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { loadModules } from 'esri-loader';

import { UiService } from '../../../common'
import { ArcGisService } from '../../arcgis.service';
import { DataServiceModel, DataServiceService } from '../dataservices.service';

@Component({
    selector: 'app-dataservices-preview-featureset',
    template: '<div #mapEl class="mapview"></div>',
    styles: [
        ':host { display: flex; height: 100%; }',
        'div.mapview { flex: 1; }'
    ]
})
export class PreviewFeatureSetComponent implements AfterViewInit, OnDestroy {
    @Input()
    public ds: DataServiceModel = { id: '' };
    @Output() public downloadProgress = new EventEmitter<number>(true);
    @ViewChild('mapEl')
    public mapElRef!: ElementRef<HTMLDivElement>;

    private mapview?: __esri.MapView;

    constructor(
        private vm: DataServiceService,
        private ui: UiService,
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
        if (count <= 0) {
            this.ui.showAlert({ type: 'warning', message: '该数据源无数据！' });
        }
        const columns = await this.vm.getColumns(id);
        const fields = columns.filter(
            x => x.name !== this.ds.geometryColumn
        );
        const json: any = await this.vm.getFeatureSetJson(
            id,
            {
                $take: count,
                $returnExtent: true,
                $select: fields.map(x => x.name ).join(',')
            },
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
            objectIdField: json.objectIdFieldName,
            popupEnabled: true,
            popupTemplate: {
                title: `{${this.ds.displayColumn}}`,
                content: [
                    {
                        type: 'fields',
                        fieldInfos: fields.map(x => {
                            return {
                                fieldName: x.name,
                                label: x.name
                            };
                        })
                    }
                ]
            }
        });
        this.mapview?.map.add(featureLayer);
        await this.mapview?.goTo(extent);
    }

}
