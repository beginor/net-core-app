import {
    Component, ElementRef, Inject, Input, OnDestroy, AfterViewInit, ViewChild,
    Output, EventEmitter
} from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
    AnyLayer, CameraOptions, CircleLayer, Map, ScaleControl, NavigationControl,
    LngLatBoundsLike, Popup
} from 'mapbox-gl';

import { lastValueFrom } from 'rxjs';

import { AccountService } from 'app-shared';
import { UiService } from '../../../common';
import { DataServiceModel, DataServiceService } from '../dataservices.service';
import { MapboxService } from '../../mapbox.service';
import * as mapboxgl from 'mapbox-gl';

@Component({
    selector: 'app-dataservices-preview-mvt',
    template: `
      <div #mapEl class="mapview"></div>
    `,
    styles: [
        ':host { display: flex; height: 100%; }',
        'div.mapview { flex: 1; }'
    ]
})
export class PreviewMvtComponent implements AfterViewInit, OnDestroy {

    @Input()
    public ds: DataServiceModel = { id: '' };
    @ViewChild('mapEl')
    public mapElRef!: ElementRef<HTMLDivElement>;

    private map?: Map;
    private popup?: Popup;

    constructor(
        private http: HttpClient,
        @Inject(DOCUMENT) private doc: Document,
        private account: AccountService,
        private ui: UiService,
        private vm: DataServiceService,
        private mapbox: MapboxService
    ) { }

    public async ngAfterViewInit(): Promise<void> {
        const map = await this.mapbox.createPreviewMap(
            this.mapElRef.nativeElement
        );
        this.map = map;
        const bounds = await this.getLayerBounds();
        await map.once('load');
        const camera = map.cameraForBounds(bounds);
        if (!!camera) {
            let zoom = camera.zoom;
            if (!!this.ds.mvtMinZoom) {
                zoom = Math.max(zoom, this.ds.mvtMinZoom)
            }
            if (!!this.ds.mvtMaxZoom) {
                zoom = Math.min(zoom, this.ds.mvtMaxZoom)
            }
            map.flyTo({
                zoom: zoom,
                center: camera.center
            });
        }
        // await map.once('idle');
        map.addSource(
            this.ds.id,
            {
                type: 'vector',
                tiles: [this.vm.getPreviewUrl(this.ds.id, 'mvt')],
                minzoom: this.ds.mvtMinZoom,
                maxzoom: this.ds.mvtMaxZoom,
                bounds: [
                    bounds.getWest(),
                    bounds.getSouth(),
                    bounds.getEast(),
                    bounds.getNorth()
                ]
            }
        );
        const layer = await this.mapbox.createPreviewLayer(
            'Polygon',
            this.ds.id,
            this.ds.id,
        );
        layer.minzoom = this.ds.mvtMinZoom;
        layer.maxzoom = this.ds.mvtMaxZoom;
        layer['source-layer'] = this.ds.name;
        map.addLayer(layer as AnyLayer);
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

    private async getLayerBounds(): Promise<mapboxgl.LngLatBounds> {
        const layerUrl = this.vm.getPreviewUrl(this.ds.id, 'mapserver');
        const params = {
            outSR: 4326,
            returnExtentOnly: true
        };
        const headers = {
            Authorization: `Bearer ${this.account.token}`
         };
        const result = await lastValueFrom(
            this.http.get<{ extent: __esri.Extent}>(
                `${layerUrl}/query`,
                { params, headers }
            )
        );
        const ext = result.extent;
        return mapboxgl.LngLatBounds.convert(
            [ext.xmin, ext.ymin, ext.xmax, ext.ymax]
        );
    }
}
