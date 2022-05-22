import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { DataApiService } from '../dataapis.service';

@Component({
    selector: 'app-dataapi-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.css']
})
export class PreviewComponent {

    public id = '';
    public title = '';
    public hasGeoColumn = false;

    constructor(
        public activeModal: NgbActiveModal,
        private vm: DataApiService
    ) { }

}
