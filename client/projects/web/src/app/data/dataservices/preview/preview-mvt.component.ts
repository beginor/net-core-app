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

import { lastValueFrom } from 'rxjs';

import { UiService } from '../../../common';
import { DataServiceModel, DataServiceService } from '../dataservices.service';
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
        private ui: UiService,
        private vm: DataServiceService,
        private mapbox: MapboxService
    ) { }

    public async ngAfterViewInit(): Promise<void> {
        const map = await this.mapbox.createPreviewMap(
            this.mapElRef.nativeElement
        );
        this.map = map;
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
