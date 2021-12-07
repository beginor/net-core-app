import { Component, Input } from '@angular/core';

import { UiService } from '../../../common';
import {
    DataApiColumnModel as ColumnModel,
    DataApiParameterModel as ParameterModel
} from '../dataapis.service';

@Component({
    selector: 'app-dataapi-param-cols',
    templateUrl: './param-cols.component.html',
    styleUrls: ['./param-cols.component.scss']
})
export class ParamColsComponent {

    @Input()
    public parameters: ParameterModel[] = [];
    @Input()
    public columns: ColumnModel[] = [];

    public paramEditIndex = -1;
    public showNewParamRow = false;
    public columnEditIndex = -1;

    public newParam: ParameterModel = { type: 'string' };
    public paramTypes = [
        'string', 'int', 'long', 'float', 'double', 'datetime', 'bool',
        'string[]', 'int[]', 'long[]', 'float[]', 'double[]', 'datetime[]',
        'bool[]'
    ];


    constructor(private ui: UiService) { }

    public addParameter(): void {
        if (!this.newParam.name) {
            this.ui.showAlert({ type: 'danger', message: '请输入参数名称！' });
            return;
        }
        if (!this.parameters) {
            this.parameters = [];
        }
        this.parameters.push(this.newParam);
        this.newParam = { type: 'string' };
        this.showNewParamRow = false;
    }

    public removeParameter(idx: number): void {
        this.parameters.splice(idx, 1);
    }

}
