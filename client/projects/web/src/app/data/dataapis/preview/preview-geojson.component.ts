import {
    Component, ElementRef, Input, OnDestroy, AfterViewInit, ViewChild,
    Output, EventEmitter
} from '@angular/core';
import { Map, LngLatBoundsLike, Popup, GeoJSONSource } from 'mapbox-gl';
import bbox from '@turf/bbox';

import { UiService } from '../../../common';
import { MapboxService } from '../../mapbox.service';

@Component({
    selector: 'app-dataapis-preview-geojson',
    template: `
      <div #mapEl class="mapview"></div>
    `,
    styles: [
        ':host { display: flex; height: 100%; }',
        'div.mapview { flex: 1; }'
    ]
})
export class PreviewGeoJsonComponent implements AfterViewInit, OnDestroy {

    @Output()
    public mapLoaded = new EventEmitter<Map>();

    @ViewChild('mapEl')
    public mapElRef!: ElementRef<HTMLDivElement>;

    private map?: Map;
    private popup?: Popup;
    private _mapLoaded = false;

    constructor(
        private ui: UiService,
        private mapbox: MapboxService
    ) { }

    public async ngAfterViewInit(): Promise<void> {
        const map = await this.mapbox.createPreviewMap(
            this.mapElRef.nativeElement
        );
        this.map = map;
        await map.once('load');
        this._mapLoaded = true;
        this.mapLoaded.next(map);
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

    public setData(geojson: GeoJSON.FeatureCollection): void {
        if (this._mapLoaded) {
            void this.addLayer(this.map!, geojson);
        }
        else {
            this.mapLoaded.subscribe(map => {
                void this.addLayer(map, geojson);
            });
        }
    }

    private async addLayer(
        map: Map,
        geojson: GeoJSON.FeatureCollection
    ): Promise<void> {
        if (geojson?.features?.length <= 0) {
            this.ui.showAlert({
                type: 'warning', message: '无数据，请检查参数！'
            });
            return;
        }
        const id = 'dataapi-geojson-preview';
        const source = map.getSource(id) as GeoJSONSource;
        if (!source) {
            map.addSource(id, { type: 'geojson', data: geojson });
        }
        else {
            source.setData(geojson);
        }
        const geoType = geojson.features[0].geometry.type;
        const fields = Object.keys(geojson.features[0].properties as any);
        const layer = await this.mapbox.createPreviewLayer(geoType, id, id);
        map.addLayer(layer as any);
        const extent = bbox(geojson);
        map.fitBounds(extent as any, { padding: 20 });
        map.on('click', id, e => {
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
                for (const field of fields) {
                    html.push(`<tr><td>${field}</td><td>${feature.properties?.[field]}</td></tr>`); // eslint-disable-line max-len
                }
                html.push('</tbody></table></div>');
                this.popup.setHTML(html.join(''));
                this.popup.addTo(this.map as Map);
            }
        });
    }
}
