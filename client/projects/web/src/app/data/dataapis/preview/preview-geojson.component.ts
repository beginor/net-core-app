import {
    Component, ElementRef, Input, OnDestroy, AfterViewInit, ViewChild,
    Output, EventEmitter
} from '@angular/core';
import { Map, LngLatBoundsLike, Popup } from 'mapbox-gl';

import { MapboxService } from '../../mapbox.service';

@Component({
    selector: 'app-dataapis-preview-geojson',
    template: `
      <div #mapEl class="mapview"></div>
    `,
    styles: [
        ':host { display: flex; height: 100%; }',
        'div.mapview { flex: 1; }'
    ]
})
export class PreviewGeoJsonComponent implements AfterViewInit, OnDestroy {

    @Output()
    public mapapLoaded = new EventEmitter<Map>();

    @ViewChild('mapEl')
    public mapElRef!: ElementRef<HTMLDivElement>;

    private map?: Map;
    private popup?: Popup;
    private fields: string[] = [];

    public setData(data: GeoJSON.FeatureCollection): void {
        //
    }

    constructor(
        private mapbox: MapboxService
    ) { }

    public async ngAfterViewInit(): Promise<void> {
        const map = await this.mapbox.createPreviewMap(
            this.mapElRef.nativeElement
        );
        this.map = map;
        await map.once('load');
        this.mapapLoaded.next(map);
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
}
