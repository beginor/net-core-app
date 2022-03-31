import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';

import {
    DataServiceService, DataServiceModel, PreviewType
} from '../dataservices.service';

@Component({
    selector: 'app-dataservices-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.scss']
})
export class PreviewComponent {

    private pvType: PreviewType = 'data';

    public ds: DataServiceModel = { id: '' };
    public get previewType(): PreviewType {
        return this.pvType;
    }
    public set previewType(val: PreviewType) {
        this.pvType = val;
        this.downloadProgress = 0;
    }
    public downloadProgress = 0;

    constructor(
        public activeModal: NgbActiveModal,
        public account: AccountService,
        public vm: DataServiceService
    ) { }

    public getPreviewUrl(): string {
        if (!this.ds.id) {
            return '';
        }
        return this.vm.getPreviewUrl(this.ds.id, this.previewType);
    }

    public getPreviewButtonClass(type: PreviewType): string {
        return this.previewType === type
            ? 'btn btn-primary'
            : 'btn btn-outline-primary';
    }

    public updateProgress(percent: number): void {
        this.downloadProgress = percent * 100;
    }

    public hasPrivilege(privilege: string): boolean {
        var info = this.account.info.getValue();
        return info.privileges[privilege];
    }

    public canShowProgress(): boolean {
        return this.previewType === 'geojson'
            || this.previewType === 'featureset';
    }

}
