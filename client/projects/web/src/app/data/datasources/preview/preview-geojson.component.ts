import {
    Component, ElementRef, Inject, Input, OnDestroy, AfterViewInit, ViewChild,
    Output, EventEmitter
} from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import {
    AnyLayer, CameraOptions, CircleLayer, Map, ScaleControl, NavigationControl,
    LngLatBoundsLike, Popup
} from 'mapbox-gl';


import { UiService } from '../../../common';
import { DataSourceModel, DataSourceService } from '../datasources.service';

@Component({
    selector: 'app-datasources-preview-geojson',
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
    public ds: DataSourceModel = { id: '' };
    @Output() public downloadProgress = new EventEmitter<number>(true);
    @ViewChild('mapEl')
    public mapElRef!: ElementRef<HTMLDivElement>;

    private map?: Map;

    private styleId = 'mapbox-gl-style';
    private geoJson?: GeoJSON.FeatureCollection;
    private options!: MapboxGlOptions;
    private popup?: Popup;
    private fields: string[] = [];

    constructor(
        private http: HttpClient,
        @Inject(DOCUMENT) private doc: Document,
        private ui: UiService,
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
            if (!!this.popup) {
                this.popup.remove();
            }
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
        const id = this.ds.id as string;
        if (geoType === 'Point' || geoType === 'MultiPoint') {
            const circle = this.options.defaults['circle'] as CircleLayer;
            circle.id = id;
            circle.source = id;
            this.map.addLayer(circle);
        }
        if (geoType === 'LineString' || geoType === 'MultiLineString') {
            const line = this.options.defaults['line'] as CircleLayer;
            line.id = id;
            line.source = id;
            this.map.addLayer(line);
        }
        if (geoType === 'Polygon' || geoType === 'MultiPolygon') {
            const fill = this.options.defaults['fill'] as CircleLayer;
            fill.id = id;
            fill.source = id;
            this.map.addLayer(fill);
        }
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
                    '<table class="table table-bordered table-striped table-sm m-0"><tbody>',
                ];
                for (const field of this.fields) {
                    html.push(`<tr><td>${field}</td><td>${feature.properties?.[field]}</td></tr>`);
                }
                html.push(`</tbody></table></div>`);
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
        const columns = await this.vm.getColumns(id);
        this.fields = columns.filter(
            x => x.name !== this.ds.geometryColumn
        ).map(x => x.name);
        if (count <= 0) {
            this.ui.showAlert({ type: 'warning', message: '该数据源无数据！' });
        }
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
