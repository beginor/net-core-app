import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DOCUMENT } from '@angular/common';
import { lastValueFrom } from 'rxjs';
import {
    CameraOptions, Layer, Map, NavigationControl, ScaleControl,
    RequestParameters
} from 'mapbox-gl';

import { AccountService } from 'app-shared';

import { OptionsService, MapboxGlOptions } from './options.service';

@Injectable({ providedIn: 'root' })
export class MapboxService {

    private styleId = 'mapbox-gl-style';
    private mapboxOptions!: MapboxGlOptions;
    private layerTypeMap: { [key: string]: string } = {
        point: 'circle',
        multipoint: 'circle',
        linestring: 'line',
        multilinestring: 'line',
        polygon: 'fill',
        multipolygon: 'fill'
    };

    constructor(
        private http: HttpClient,
        @Inject('webRoot') private webRoot: string,
        private account: AccountService,
        @Inject(DOCUMENT) private doc: Document,
        private options: OptionsService
    ) { }

    public async createPreviewMap(container: HTMLElement): Promise<Map> {
        await this.loadMapCss();
        await this.loadMapOptions();
        const map = new Map({
            accessToken: this.mapboxOptions?.accessToken,
            style: this.mapboxOptions?.style,
            center: this.mapboxOptions?.camera.center,
            zoom: this.mapboxOptions?.camera.zoom,
            pitch: this.mapboxOptions?.camera.pitch,
            bearing: this.mapboxOptions?.camera.bearing,
            container: container,
            attributionControl: false,
            transformRequest: (url): RequestParameters => {
                const params: RequestParameters = { url };
                if (url.includes(this.webRoot)) {
                    params.headers = {};
                    this.account.addAuthTokenTo(params.headers);
                }
                return params;
            }
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
        const layerType = this.layerTypeMap[geoType.toLowerCase()];
        if (!layerType) {
            throw new Error(`Invalid geometry type ${geoType}`);
        }
        const layer = this.mapboxOptions.defaults[layerType];
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
        if (!!this.mapboxOptions) {
            return;
        }
        this.mapboxOptions = await this.options.loadMapboxGlOptions();
    }
}
