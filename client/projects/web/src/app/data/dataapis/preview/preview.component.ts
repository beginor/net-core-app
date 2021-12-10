import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { DataApiService } from '../dataapis.service';

@Component({
    selector: 'app-dataapi-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.scss']
})
export class PreviewComponent {

    public id: string = '';
    public title: string = '';

    constructor(
        public activeModal: NgbActiveModal,
        private vm: DataApiService
    ) { }

}
