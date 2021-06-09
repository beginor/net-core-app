import {
    Component, ElementRef, Input, OnDestroy, AfterViewInit, ViewChild,
    Output, EventEmitter
} from '@angular/core';

import { loadModules } from 'esri-loader';

import { DataServiceModel, DataServiceService } from '../dataservices.service';
import { ArcGisService } from '../../arcgis.service';

@Component({
    selector: 'app-dataservices-preview-mapserver',
    template: `<div #mapEl class="mapview"></div>`,
    styles: [
        ':host { display: flex; height: 100%; }',
        'div.mapview { flex: 1; }'
    ]
})
export class PreviewMapServerComponent implements AfterViewInit, OnDestroy {
    @Input()
    public ds: DataServiceModel = { id: '' };
    @Output() public downloadProgress = new EventEmitter<number>(true);
    @ViewChild('mapEl')
    public mapElRef!: ElementRef<HTMLDivElement>;

    private mapview?: __esri.MapView;

    constructor(
        private vm: DataServiceService,
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
        const columns = await this.vm.getColumns(id);
        const fields = columns.filter(
            x => x.name !== this.ds.geometryColumn
        );
        const [FeatureLayer] = await loadModules<[
            __esri.FeatureLayerConstructor
        ]>([
            'esri/layers/FeatureLayer'
        ]);
        const featureLayer = new FeatureLayer({
            url: this.vm.getPreviewUrl(id, 'mapserver'),
            outFields: fields.map(x => x.name),
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
        await featureLayer.load();
        // await featureLayer.when();
        this.mapview?.map.add(featureLayer);
        this.mapview?.goTo(featureLayer.fullExtent);
    }

}
