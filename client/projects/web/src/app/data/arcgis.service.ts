import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { lastValueFrom } from 'rxjs';

import { AccountService } from 'app-shared';

import {
    isLoaded, loadScript, loadModules, ILoadScriptOptions
} from 'esri-loader';

@Injectable({
    providedIn: 'root'
})
export class ArcGisService {

    private opts!: ArcGisJsApiOptions;

    constructor(
        private http: HttpClient,
        @Inject('webRoot') private webRoot: string,
        private account: AccountService
    ) { }

    public async loadJsApi(): Promise<void> {
        if (isLoaded()) {
            return;
        }
        const opts = await lastValueFrom(this.http.get<ArcGisJsApiOptions>(
            'assets/arcgis-js-api.options.json'
        ));
        this.opts = opts;
        const dojoConfig = opts.dojoConfig;
        Object.assign(window, { dojoConfig });
        const esriConfig = opts.esriConfig;
        if (!esriConfig.request.interceptors) {
            esriConfig.request.interceptors = [];
        }
        esriConfig.request.interceptors.push({
            urls: new RegExp(this.webRoot),
            before: param => {
                const headers = param.requestOptions.headers || {};
                headers['X-Requested-With'] = 'XMLHttpRequest';
                headers['Authorization'] = `Bearer ${this.account.token}`
                param.requestOptions.headers = headers;
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
        const mapView = await this.createMapView(container);
        await mapView.when();
        const [TileLayer] = await loadModules<[
            __esri.TileLayerConstructor
        ]>([
            'esri/layers/TileLayer'
        ]);
        const tileLayer = new TileLayer({ url });
        await tileLayer.load();
        mapView.map.add(tileLayer);
        await mapView.goTo(tileLayer.fullExtent);
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
            map, container,
            center: this.opts.center,
            zoom: this.opts.zoom
        });
        const fullscreen = new Fullscreen({ view: mapView });
        mapView.ui.add(fullscreen, 'top-right');
        await mapView.when();
        map.add(meshLayer);
        await mapView.goTo(meshLayer.fullExtent);
        return mapView;
    }

    public async createMapView(
        container: HTMLDivElement
    ): Promise<__esri.MapView> {
        if (!isLoaded()) {
            await this.loadJsApi();
        }
        const [Map, MapView, Fullscreen] = await loadModules<[
            __esri.MapConstructor,
            __esri.MapViewConstructor,
            __esri.FullscreenConstructor,
        ]>([
            'esri/Map',
            'esri/views/MapView',
            'esri/widgets/Fullscreen'
        ]);
        const mapview = new MapView({
            container,
            map: new Map({ basemap: 'satellite' }),
            center: this.opts.center,
            zoom: this.opts.zoom
        });
        mapview.ui.add(new Fullscreen({view: mapview}), 'top-right');
        return mapview;
    }

    public async createVectorTileLayerPreview(
        container: HTMLDivElement,
        url: string
    ): Promise<__esri.MapView> {
        const mapview = await this.createMapView(container);
        await mapview.when();
        const [VectorTileLayer, Extent] = await loadModules<[
            __esri.VectorTileLayerConstructor,
            __esri.ExtentConstructor
        ]>([
            'esri/layers/VectorTileLayer',
            'esri/geometry/Extent'
        ]);
        const style: any = await lastValueFrom(
            this.http.get(url, { headers: { ['Authorization']: `Bearer ${this.account.token}` } }) // eslint-disable-line max-len
        );
        const vectorTileLayer = new VectorTileLayer({ style });
        mapview.map.add(vectorTileLayer);
        const extent: number[] = style.metadata?.['mapbox:extent'];
        if (!!extent) {
            await mapview.goTo(new Extent({
                xmin: extent[0],
                ymin: extent[1],
                xmax: extent[2],
                ymax: extent[3],
                spatialReference: { wkid: 4326 }
            }));
        }
        return mapview;
    }

}

interface ArcGisJsApiOptions extends ILoadScriptOptions {
    dojoConfig: Object;
    esriConfig: __esri.config;
    center: number[];
    zoom: number;
}
