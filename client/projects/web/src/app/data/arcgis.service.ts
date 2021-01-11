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
        const dojoConfig = opts.dojoConfig;
        Object.assign(window, { dojoConfig });
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

    public async createTileLayerPreview(
        container: HTMLDivElement,
        url: string
    ): Promise<__esri.MapView> {
        if (!isLoaded()) {
            await this.loadJsApi();
        }
        const [Map, MapView, TileLayer, Fullscreen] = await loadModules<[
            __esri.MapConstructor,
            __esri.MapViewConstructor,
            __esri.TileLayerConstructor,
            __esri.FullscreenConstructor
        ]>([
            'esri/Map',
            'esri/views/MapView',
            'esri/layers/TileLayer',
            'esri/widgets/Fullscreen'
        ]);
        const tileLayer = new TileLayer({ url });
        await tileLayer.load();
        const map = new Map({
            layers: [tileLayer]
        });
        const mapView = new MapView({
            map, container, extent: tileLayer.fullExtent
        });
        const fullscreen = new Fullscreen({ view: mapView });
        mapView.ui.add(fullscreen, 'top-right');
        mapView.when().then(() => {
            mapView.goTo(tileLayer.fullExtent);
        });
        return mapView;
    }

    public async createSlpkLayerPreview(
        container: HTMLDivElement,
        url: string
    ): Promise<__esri.SceneView> {
        if (!isLoaded()) {
            await this.loadJsApi();
        }
        const [WebScene, SceneView, MeshLayer, Fullscreen] = await loadModules<[
            __esri.WebSceneConstructor,
            __esri.SceneViewConstructor,
            __esri.IntegratedMeshLayerConstructor,
            __esri.FullscreenConstructor
        ]>([
            'esri/WebScene',
            'esri/views/SceneView',
            'esri/layers/IntegratedMeshLayer',
            'esri/widgets/Fullscreen'
        ]);
        const meshLayer = new MeshLayer({ url });
        await meshLayer.load();
        const map = new WebScene({
            basemap: 'satellite',
            ground: 'world-elevation'
        });
        const mapView = new SceneView({
            map, container, extent: meshLayer.fullExtent
        });
        const fullscreen = new Fullscreen({ view: mapView });
        mapView.ui.add(fullscreen, 'top-right');
        mapView.when().then(async () => {
            map.add(meshLayer);
            mapView.goTo(meshLayer.fullExtent);
        });
        return mapView;
    }

}

interface ArcGisJsApiOptions extends ILoadScriptOptions {
    dojoConfig: Object;
    esriConfig: __esri.config;
}
