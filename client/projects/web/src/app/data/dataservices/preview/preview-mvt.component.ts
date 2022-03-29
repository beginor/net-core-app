import {
    Component, ElementRef, Inject, Input, OnDestroy, AfterViewInit, ViewChild
} from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { HttpClient} from '@angular/common/http';
import { AnyLayer, Map, Popup, LngLatBounds } from 'mapbox-gl';

import { lastValueFrom } from 'rxjs';

import { AccountService } from 'app-shared';
import { UiService } from '../../../common';
import { DataServiceModel, DataServiceService, MvtInfo } from '../dataservices.service';
import { MapboxService } from '../../mapbox.service';

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
        await map.once('load');
        map.resize();
        const mvtInfo = await this.vm.getMvtInfo(this.ds.id);
        if (!mvtInfo) {
            return;
        }
        this.addVectorTileLayer(mvtInfo);
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

    private async addVectorTileLayer(mvtInfo: MvtInfo): Promise<void> {
        if (!this.map) {
            return;
        }
        const bounds = mvtInfo.bounds;
        this.map.flyTo({
            zoom: mvtInfo.minzoom,
            center: [(bounds[0] + bounds[2])/2.0, (bounds[1] + bounds[3])/2.0]
        });
        this.map.addSource(
            this.ds.id,
            {
                type: 'vector',
                tiles: [mvtInfo.url],
                minzoom: this.ds.mvtMinZoom,
                maxzoom: this.ds.mvtMaxZoom,
                bounds: mvtInfo.bounds
            }
        );
        const layer = await this.mapbox.createPreviewLayer(
            mvtInfo.geometryType,
            this.ds.id,
            this.ds.id,
        );
        layer.minzoom = this.ds.mvtMinZoom;
        layer.maxzoom = this.ds.mvtMaxZoom;
        layer['source-layer'] = this.ds.name;
        this.map.addLayer(layer as AnyLayer);
        this.map.on('click', this.ds.id, e => {
            if (!e.features || e.features.length === 0) {
                return;
            }
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
            const fields = Object.keys(feature.properties as any)
            for (const field of fields) {
                html.push(`<tr><td>${field}</td><td>${feature.properties?.[field]}</td></tr>`); // eslint-disable-line max-len
            }
            html.push('</tbody></table></div>');
            this.popup.setHTML(html.join(''));
            this.popup.addTo(this.map as Map);
        });
    }
}
