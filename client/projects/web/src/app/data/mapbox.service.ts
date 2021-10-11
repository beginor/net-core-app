import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DOCUMENT } from '@angular/common';
import { lastValueFrom } from 'rxjs';
import {
    CameraOptions, Layer, Map, NavigationControl, ScaleControl
} from 'mapbox-gl';

import { AccountService } from 'app-shared';

@Injectable({ providedIn: 'root' })
export class MapboxService {

    private styleId = 'mapbox-gl-style';
    private options!: MapboxGlOptions;
    private layerTypeMap: { [key: string]: string } = {
        Point: 'circle',
        MultiPoint: 'circle',
        LineString: 'line',
        MultiLineString: 'line',
        Polygon: 'fill',
        MultiPolygon: 'fill'
    };

    constructor(
        private http: HttpClient,
        @Inject('webRoot') private webRoot: string,
        private account: AccountService,
        @Inject(DOCUMENT) private doc: Document,
    ) { }

    public async createPreviewMap(container: HTMLElement): Promise<Map> {
        await this.loadMapCss();
        await this.loadMapOptions();
        const map = new Map({
            accessToken: this.options?.accessToken,
            style: this.options?.style,
            center: this.options?.camera.center,
            zoom: this.options?.camera.zoom,
            pitch: this.options?.camera.pitch,
            bearing: this.options?.camera.bearing,
            container: container,
            attributionControl: false
        });
        map.addControl(new NavigationControl(), 'top-left');
        map.addControl(new ScaleControl(), 'bottom-right');
        return map;
    }

    public destroyMap(map: Map): void {
        map.remove();
        this.removeMapCss();
    }

    public async createPreviewLayer(
        geoType: string,
        layerId: string,
        sourceId: string
    ): Promise<Layer> {
        await this.loadMapOptions();
        const layerType = this.layerTypeMap[geoType];
        if (!layerType) {
            throw new Error(`Invalid geometry type ${geoType}`);
        }
        const layer = this.options.defaults[layerType];
        if (!layer) {
            throw new Error(`Default style for ${layerType} is not defined!`);
        }
        layer.id = layerId;
        layer.source = sourceId;
        return layer;
    }
    private loadMapCss(): Promise<void> {
        const style = this.doc.head.querySelector(`#${this.styleId}`);
        if (!!style) {
            return Promise.resolve();
        }
        return new Promise((resolve, reject) => {
            const styleLink = this.doc.createElement('link');
            styleLink.setAttribute('rel', 'stylesheet');
            styleLink.setAttribute('id', this.styleId);
            styleLink.setAttribute('href', 'mapbox-gl.css');
            this.doc.head.appendChild(styleLink);
            styleLink.onload = (): void => {
                setTimeout(() => resolve(), 300);
            };
            styleLink.onerror = (ex): void => reject(ex);
        });
    }

    private removeMapCss(): void {
        const style = this.doc.head.querySelector(`#${this.styleId}`);
        if (!!style) {
            style.remove();
        }
    }

    private async loadMapOptions(): Promise<void> {
        if (!!this.options) {
            return;
        }
        this.options = await lastValueFrom(
            this.http.get<MapboxGlOptions>('./assets/mapbox-gl.options.json')
        );
    }
}

export interface MapboxGlOptions {
    accessToken: string;
    style: string;
    camera: CameraOptions;
    defaults: { [key: string]: Layer };
}
