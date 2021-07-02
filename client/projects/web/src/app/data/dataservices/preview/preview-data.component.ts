import { Component, Input, OnInit } from '@angular/core';

import {
    DataServiceModel, DataServiceService, ReadDataParam, DataServiceFieldModel
} from '../dataservices.service';

@Component({
    selector: 'app-dataservices-preview-data',
    template: `
      <table class="table table-striped table-sm">
        <thead>
          <ng-container *ngIf="loadingColumns;else colsTpl">
            <tr>
              <th class="text-center text-warning">正在加载字段列表...</th>
            </tr>
          </ng-container>
          <ng-template #colsTpl>
            <tr>
              <th scope="col" *ngFor="let col of columns" >
                <div *ngIf="!!col.description" class="text-muted text-nowrap">{{col.description}}</div>
                <div class="text-body text-nowrap">{{col.name}}</div>
              </th>
            </tr>
          </ng-template>
        </thead>
        <tbody>
          <ng-container *ngIf="loadingData;else dataTpl">
            <tr>
              <td class="text-warning text-center" [colSpan]="columns.length" >正在加载数据！</td>
            </tr>
          </ng-container>
          <ng-template #dataTpl>
            <ng-container *ngIf="isEmpty;else rowsTpl">
              <tr>
                <td class="text-warning text-center" [colSpan]="columns.length" >该数据源无数据！</td>
              </tr>
            </ng-container>
          </ng-template>
          <ng-template #rowsTpl>
            <tr *ngFor="let row of data; let i = index;">
              <td class="text-body text-nowrap" *ngFor="let col of columns">
              {{row[col.name]}}
              </td>
            </tr>
            <tr *ngIf="total > data.length">
              <td class="table-info" [colSpan]="columns.length">
              共 {{total}} 条记录， 预览仅显示前 {{data.length}} 条记录。
              </td>
            </tr>
          </ng-template>
        </tbody>
      </table>
    `,
    styles: [
        ':host { display: flex; height: 100%; }',
        '.table { flex: 1 }',
        '.col-name, .col-desc { white-space: nowrap; }'
    ]
})
export class PreviewDataComponent implements OnInit {

    @Input()
    public ds: DataServiceModel = { id: '' };
    public columns: DataServiceFieldModel[] = [];
    public data: any[] = [];
    public total = 0;
    public readDataParam: ReadDataParam = { };
    public loadingColumns = false;
    public loadingData = false;
    public isEmpty = false;

    constructor(
        private vm: DataServiceService
    ) { }

    public async ngOnInit(): Promise<void> {
        const id = this.ds.id;
        if (!id) {
            return;
        }
        await this.loadColumns();
        await this.loadData();
    }

    private async loadColumns(): Promise<void> {
        this.loadingColumns = true;
        let cols = await this.vm.getColumns(this.ds.id);
        if (!!this.ds.geometryColumn) {
            cols = cols.filter(col => col.name !== this.ds.geometryColumn);
        }
        this.columns = cols;
        this.loadingColumns = false;
    }

    private async loadData(): Promise<void> {
        this.loadingData = true;
        this.readDataParam.$select = this.columns.map(
            col => col.name
        ).join(',');
        this.readDataParam.$take = 20;
        const result = await this.vm.getData(this.ds.id, this.readDataParam);
        this.data = result.data ?? [];
        this.total = result.total ?? 0;
        this.loadingData = false;
        this.isEmpty = this.total === 0;
    }

}
