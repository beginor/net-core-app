import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { AccountService } from 'app-shared';

import { isLoaded, loadScript, loadModules, ILoadScriptOptions } from 'esri-loader';

@Injectable({
    providedIn: 'root'
})
export class ArcGisService {

    constructor(
        private http: HttpClient,
        @Inject('webRoot') private webRoot: string,
        private account: AccountService
    ) { }

    public async loadJsApi(): Promise<void> {
        if (isLoaded()) {
            return;
        }
        const opts = await this.http.get<ArcGisJsApiOptions>(
            'assets/arcgis-js-api.options.json'
        ).toPromise();
        const esriConfig = opts.esriConfig;
        if (!esriConfig.request.interceptors) {
            esriConfig.request.interceptors = [];
        }
        esriConfig.request.interceptors.push({
            urls: new RegExp(this.webRoot),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                Authorization: `Bearer ${this.account.token}`
            }
        });
        Object.assign(window, { esriConfig });
        // delete opts.esriConfig;
        await loadScript(opts);
    }

    public async createLayerPreview(
        container: HTMLDivElement,
        url: string
    ): Promise<__esri.MapView> {
        const [Map, MapView, TileLayer] = await loadModules<[
            __esri.MapConstructor,
            __esri.MapViewConstructor,
            __esri.TileLayerConstructor
        ]>([
            'esri/Map',
            'esri/views/MapView',
            'esri/layers/TileLayer'
        ]);
        const tileLayer = new TileLayer({ url });
        const map = new Map({
            layers: [tileLayer]
        });
        const mapView = new MapView({
            map, container
        });
        mapView.when().then(() => {
            mapView.goTo(tileLayer.fullExtent);
        });
        return mapView;
    }

}

interface ArcGisJsApiOptions extends ILoadScriptOptions {
    esriConfig: __esri.config;
}
