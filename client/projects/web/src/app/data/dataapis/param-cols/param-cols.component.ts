import { Component, Input, OnInit } from '@angular/core';

import { UiService } from '../../../common';
import {
    DataApiColumnModel as ColumnModel,
    DataApiParameterModel as ParameterModel,
    DataApiModel
} from '../dataapis.service';

@Component({
    selector: 'app-dataapi-param-cols',
    templateUrl: './param-cols.component.html',
    styleUrls: ['./param-cols.component.scss']
})
export class ParamColsComponent {

    private apiModel!: DataApiModel;

    @Input()
    public get model(): DataApiModel {
        return this.apiModel;
    }
    public set model(val: DataApiModel) {
        this.apiModel = val;
        if (!!val?.idColumn) {
            this.hasIdCol = true;
        }
        if (!!val?.geometryColumn) {
            this.hasGeoCol = true;
        }
    }

    public paramEditIndex = -1;
    public showNewParamRow = false;
    public columnEditIndex = -1;
    public showNewColRow = false;

    public hasIdCol = false;
    public hasGeoCol = false;

    public newParam: ParameterModel = { type: 'string' };
    public paramTypes = [
        'string', 'int', 'long', 'float', 'double', 'datetime', 'bool',
        'string[]', 'int[]', 'long[]', 'float[]', 'double[]', 'datetime[]',
        'bool[]'
    ];

    public newCol: ColumnModel = { type: 'varchar' };

    constructor(private ui: UiService) { }

    public addParameter(): void {
        if (!this.newParam.name) {
            this.ui.showAlert({ type: 'danger', message: '请输入参数名称！' });
            return;
        }
        if (!this.model.parameters) {
            this.model.parameters = [];
        }
        this.model.parameters.push(this.newParam);
        this.newParam = { type: 'string' };
        this.showNewParamRow = false;
    }

    public async removeParameter(idx: number): Promise<void> {
        const confirmed = await this.ui.showConfirm('确认删除么？');
        if (confirmed) {
            this.model.parameters?.splice(idx, 1);
        }
    }

    public async removeColumn(idx: number): Promise<void> {
        const confirmed = await this.ui.showConfirm('确认删除么？');
        if (confirmed) {
            this.model.columns?.splice(idx, 1);
        }
    }

    public addNewCol(): void {
        if (!this.newCol.name) {
            this.ui.showAlert({ type: 'danger', message: '请输入字段名称!' });
            return;
        }
        if (!this.newCol.type) {
            this.ui.showAlert({ type: 'danger', message: '请输入字段类型' });
        }
        if (!this.model.columns) {
            this.model.columns = [];
        }
        this.model.columns.push(this.newCol);
        this.newCol = { type: 'varchar' };
        this.showNewColRow = false;
    }

    public checkIdCol(): void {
        if (!this.hasIdCol) {
            this.model.idColumn = undefined;
        }
    }

    public checkGeoCol(): void {
        if (!this.hasGeoCol) {
            this.model.geometryColumn = undefined;
        }
    }

}
