import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { DataSourceService, DataSourceModel, ReadDataParam, PreviewType } from '../datasources.service';
import { ColumnModel } from '../metadata.service';

@Component({
    selector: 'app-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.scss']
})
export class PreviewComponent implements OnInit {

    public ds: DataSourceModel = { };

    public columns: ColumnModel[] = [];
    public data: any[] = [];
    public readDataParam: ReadDataParam = {};
    public previewType: PreviewType = 'data';

    constructor(
        public activeModal: NgbActiveModal,
        public vm: DataSourceService
    ) { }

    public async ngOnInit(): Promise<void> {
        const id = this.ds.id as string;
        this.columns = await this.vm.getColumns(id);
        this.readDataParam.$select = this.columns.map(
            col => col.name
        ).join(',');
        const result = await this.vm.getData(
            id,
            this.readDataParam
        );
        this.data = result.data ?? [];
    }

    public setPreviewType(type: PreviewType): void {
        this.previewType = type;
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

}
