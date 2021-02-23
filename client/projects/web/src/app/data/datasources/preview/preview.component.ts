import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { DataSourceService, ReadDataParam } from '../datasources.service';
import { ColumnModel } from '../metadata.service';

@Component({
    selector: 'app-preview',
    templateUrl: './preview.component.html',
    styleUrls: ['./preview.component.scss']
})
export class PreviewComponent implements OnInit {

    @Input() public id = '';
    @Input() public name = '';

    public columns: ColumnModel[] = [];
    public data: any[] = [];
    public readDataParam: ReadDataParam = {};

    constructor(
        public activeModal: NgbActiveModal,
        public vm: DataSourceService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.columns = await this.vm.getColumns(this.id);
        this.readDataParam.$select = this.columns.map(
            col => col.name
        ).join(',');
        const result = await this.vm.getData(
            this.id,
            this.readDataParam
        );
        this.data = result.data ?? [];
    }

}
