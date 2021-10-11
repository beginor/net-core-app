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

    constructor(
        private http: HttpClient,
        @Inject(DOCUMENT) private doc: Document,
        private ui: UiService,
        private vm: DataServiceService
    ) { }

    public async ngAfterViewInit(): Promise<void> {
        console.log('ngAfterViewInit');
    }

    public ngOnDestroy(): void {
        console.log('ngOnDestroy');
    }
}
