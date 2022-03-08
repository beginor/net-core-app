import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { lastValueFrom } from 'rxjs';
import { ILoadScriptOptions } from 'esri-loader';
import { CameraOptions, Layer } from 'mapbox-gl';

@Injectable({ providedIn: 'root' })
export class OptionsService {

    private options!: Options;

    constructor(private http: HttpClient) { }

    public async loadArcGisJsApiOptions(): Promise<ArcGisJsApiOptions> {
        await this.loadOptions();
        return this.options['arcgis-js-api'];
    }

    public async loadMapboxGlOptions(): Promise<MapboxGlOptions> {
        await this.loadOptions();
        return this.options['mapbox-gl'];
    }

    private async loadOptions(): Promise<void> {
        if (!this.options) {
            const options = await lastValueFrom(
                this.http.get<Options>('./assets/options.json')
            );
            this.options = options;
        }
    }
}

export interface Options {
    'arcgis-js-api': ArcGisJsApiOptions;
    'mapbox-gl': MapboxGlOptions;
}

export interface ArcGisJsApiOptions extends ILoadScriptOptions {
    dojoConfig: Object;
    esriConfig: __esri.config;
    center: number[];
    zoom: number;
}

export interface MapboxGlOptions {
    accessToken: string;
    style: string;
    camera: CameraOptions;
    defaults: { [key: string]: Layer };
}
