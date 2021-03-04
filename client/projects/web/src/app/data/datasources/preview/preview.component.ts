import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { DataSourceService, DataSourceModel, PreviewType } from '../datasources.service';

@Component({
    selector: 'app-datasources-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.scss']
})
export class PreviewComponent implements OnInit {

    private pvType: PreviewType = 'data';

    public ds: DataSourceModel = { id: '' };
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
        public vm: DataSourceService
    ) { }

    public async ngOnInit(): Promise<void> {
    }

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

}
