import { Component, Input, OnDestroy, OnInit } from '@angular/core';

import { ColumnModel } from '../metadata.service';
import { DataSourceModel, DataSourceService, ReadDataParam } from '../datasources.service';

@Component({
    selector: 'app-datasources-preview-data',
    template: `
      <table class="table table-striped table-sm">
        <thead>
          <tr>
            <th scope="col" *ngFor="let col of columns" >
              <div *ngIf="!!col.description" class="text-muted text-nowrap">{{col.description}}</div>
              <div class="text-body text-nowrap">{{col.name}}</div>
            </th>
          </tr>
        </thead>
        <tbody>
          <ng-container *ngIf="data.length > 0;else emptyTpl">
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
          </ng-container>
          <ng-template #emptyTpl>
            <tr>
              <td class="text-warning text-center" [colSpan]="columns.length" >该数据源无数据！</td>
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
export class PreviewDataComponent implements OnInit, OnDestroy {

    @Input()
    public ds: DataSourceModel = { id: '' };
    public columns: ColumnModel[] = [];
    public data: any[] = [];
    public total = 0;
    public readDataParam: ReadDataParam = { };

    constructor(
        private vm: DataSourceService
    ) { }

    public async ngOnInit(): Promise<void> {
        const id = this.ds.id;
        if (!id) {
            return;
        }
        let cols = await this.vm.getColumns(id);
        if (!!this.ds.geometryColumn) {
            cols = cols.filter(col => col.name !== this.ds.geometryColumn);
        }
        this.columns = cols;
        this.readDataParam.$select = cols.map(col => col.name).join(',');
        this.readDataParam.$take = 20;
        const result = await this.vm.getData(id, this.readDataParam);
        this.data = result.data ?? [];
        this.total = result.total ?? 0;
    }

    public ngOnDestroy(): void {
        // alert('destroy');
    }

}
