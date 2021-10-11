import {
    Component, ElementRef, Input, OnDestroy, AfterViewInit, ViewChild,
    Output, EventEmitter
} from '@angular/core';
import { Map, LngLatBoundsLike, Popup } from 'mapbox-gl';


import { UiService } from '../../../common';
import { DataServiceModel, DataServiceService } from '../dataservices.service';
import { MapboxService } from '../../mapbox.service';

@Component({
    selector: 'app-dataservices-preview-geojson',
    template: `
      <div #mapEl class="mapview"></div>
    `,
    styles: [
        ':host { display: flex; height: 100%; }',
        'div.mapview { flex: 1; }'
    ]
})
export class PreviewGeoJsonComponent implements AfterViewInit, OnDestroy {
    @Input()
    public ds: DataServiceModel = { id: '' };
    @Output() public downloadProgress = new EventEmitter<number>(true);
    @ViewChild('mapEl')
    public mapElRef!: ElementRef<HTMLDivElement>;

    private map?: Map;
    private geoJson?: GeoJSON.FeatureCollection;
    private popup?: Popup;
    private fields: string[] = [];

    constructor(
        private ui: UiService,
        private vm: DataServiceService,
        private mapbox: MapboxService
    ) { }

    public async ngAfterViewInit(): Promise<void> {
        const map = await this.mapbox.createPreviewMap(
            this.mapElRef.nativeElement
        );
        this.map = map;
        await Promise.all([map.once('load'), this.loadGeoJson()])
        void this.addGeoJsonLayer();
    }

    public ngOnDestroy(): void {
        if (!!this.map) {
            if (!!this.popup) {
                this.popup.remove();
            }
            this.mapbox.destroyMap(this.map);
            this.map = undefined;
        }
    }

    private async addGeoJsonLayer(): Promise<void> {
        if (!this.geoJson || this.geoJson.features.length <= 0) {
            return;
        }
        if (!this.map) {
            return;
        }
        const geoType = this.geoJson.features[0].geometry.type;
        const id = this.ds.id as string;
        const layer = await this.mapbox.createPreviewLayer(geoType, id, id);
        this.map.addLayer(layer as any);
        this.map.on('click', id, e => {
            if (!!e.features && e.features.length > 0) {
                const feature = e.features[0];
                if (!this.popup) {
                    this.popup = new Popup({
                        closeButton: true,
                        closeOnClick: true
                    });
                }
                this.popup.setLngLat(e.lngLat);
                const html = [
                    '<div style="max-height: 200px; overflow: auto;">',
                    '<table class="table table-bordered table-striped table-sm m-0"><tbody>', // eslint-disable-line max-len
                ];
                for (const field of this.fields) {
                    html.push(`<tr><td>${field}</td><td>${feature.properties?.[field]}</td></tr>`); // eslint-disable-line max-len
                }
                html.push('</tbody></table></div>');
                this.popup.setHTML(html.join(''));
                this.popup.addTo(this.map as Map);
            }
        });
    }

    private async loadGeoJson(): Promise<void> {
        const id = this.ds.id;
        if (!id) {
            return;
        }
        const count = await this.vm.getCount(id, { });
        if (count <= 0) {
            this.ui.showAlert({ type: 'warning', message: '该数据源无数据！' });
        }
        const columns = await this.vm.getColumns(id);
        this.fields = columns.filter(
            x => x.name !== this.ds.geometryColumn
        ).map(x => x.name);
        const geojson = await this.vm.getGeoJson(
            id,
            {
                $take: count,
                $returnBbox: true,
                $select: this.fields.join(',')
            },
            (total, loaded) => {
                const percent = Number.parseFloat((loaded / total).toFixed(2));
                this.downloadProgress.next(percent);
            }
        );
        const bbox = geojson.bbox;
        if (!!bbox) {
            this.map?.fitBounds(bbox as LngLatBoundsLike, { padding: 20 });
        }
        this.map?.addSource(id, { type: 'geojson', data: geojson});
        this.geoJson = geojson;
    }

}
