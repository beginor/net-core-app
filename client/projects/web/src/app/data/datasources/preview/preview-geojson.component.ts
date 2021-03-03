import {
    Component, ElementRef, Inject, Input, OnDestroy, AfterViewInit, ViewChild
} from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';

import { DataSourceModel, DataSourceService } from '../datasources.service';
import {
    AnyLayer, CameraOptions, CircleLayer, Map, ScaleControl, NavigationControl
} from 'mapbox-gl';


@Component({
    selector: 'app-datasources-preview-geojson',
    template: `
      <div #mapEl class="mapview_"></div>
    `,
    styles: [
        ':host { display: flex; height: 100%; }',
        'div.mapview_ { flex: 1; }'
    ]
})
export class PreviewGeoJsonComponent implements AfterViewInit, OnDestroy {
    @Input()
    public ds: DataSourceModel = { id: '' };
    @ViewChild('mapEl')
    public mapElRef!: ElementRef<HTMLDivElement>;

    private map?: Map;

    private styleId = 'mapbox-gl-style';
    private geoJson?: GeoJSON.FeatureCollection;
    private options!: MapboxGlOptions;

    constructor(
        private http: HttpClient,
        @Inject(DOCUMENT) private doc: Document,
        private vm: DataSourceService
    ) {
    }

    public async ngAfterViewInit(): Promise<void> {
        await this.loadMapStyle();
        this.options = await this.http.get<MapboxGlOptions>('./assets/mapbox-gl.options.json').toPromise();
        const map = new Map({
            accessToken: this.options.accessToken,
            style: this.options.style,
            center: this.options.camera.center,
            zoom: this.options.camera.zoom,
            pitch: this.options.camera.pitch,
            bearing: this.options.camera.bearing,
            container: this.mapElRef.nativeElement,
            attributionControl: false
        });
        map.addControl(new NavigationControl(), 'top-left');
        map.addControl(new ScaleControl(), 'bottom-right');
        this.map = map;
        await map.once('load');
        await this.loadGeoJson();
        this.addLayer();
    }

    public ngOnDestroy(): void {
        if (!!this.map) {
            this.map.remove();
            const style = this.doc.head.querySelector(`#${this.styleId}`);
            if (!!style) {
                style.remove();
            }
            this.map = undefined;
        }
    }

    private addLayer(): void {
        if (!this.geoJson || this.geoJson.features.length <= 0) {
            return;
        }
        if (!this.map) {
            return;
        }
        const geoType = this.geoJson.features[0].geometry.type;
        if (geoType === 'Point' || geoType === 'MultiPoint') {
            const circle = this.options.defaults['circle'] as CircleLayer;
            circle.id = this.ds.id as string;
            circle.source = this.ds.id as string;
            this.map.addLayer(circle);
        }
        if (geoType === 'LineString' || geoType === 'MultiLineString') {
            const line = this.options.defaults['line'] as CircleLayer;
            line.id = this.ds.id as string;
            line.source = this.ds.id as string;
            this.map.addLayer(line);
        }
        if (geoType === 'Polygon' || geoType === 'MultiPolygon') {
            const fill = this.options.defaults['fill'] as CircleLayer;
            fill.id = this.ds.id as string;
            fill.source = this.ds.id as string;
            this.map.addLayer(fill);
        }
    }

    private async loadGeoJson(): Promise<void> {
        const id = this.ds.id;
        if (!id) {
            return;
        }
        const count = await this.vm.getCount(id, { });
        const geojson = await this.vm.getGeoJson(id, { $take: count });
        this.map?.addSource(id, { type: 'geojson', data: geojson as any});
        this.geoJson = geojson;
    }

    private loadMapStyle(): Promise<void> {
        const doc = this.doc;
        const head = this.doc.head;
        return new Promise((resolve, reject) => {
            const styleLink = doc.createElement('link');
            styleLink.setAttribute('rel', 'stylesheet');
            styleLink.setAttribute('id', this.styleId);
            styleLink.setAttribute('href', 'mapbox-gl.css');
            head.appendChild(styleLink);
            styleLink.onload = () => {
                setTimeout(() => resolve(), 300);
            };
            styleLink.onerror = (ex) => reject(ex);
        });
    }

}

interface MapboxGlOptions {
    accessToken: string;
    style: string;
    camera: CameraOptions;
    defaults: { [key: string]: AnyLayer };
}
