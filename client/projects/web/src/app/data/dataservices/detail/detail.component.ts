import { Component, OnInit, ViewChild } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable, Subject, merge } from 'rxjs';
import {
    debounceTime, distinctUntilChanged, filter, map
} from 'rxjs/operators';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';

import {
    DataServiceService, DataServiceModel, DataServiceFieldModel
} from '../dataservices.service';
import { MetadataService, TableModel, ColumnModel } from '../metadata.service';
import {
    DataSourceService, DataSourceModel
} from '../../datasources/datasources.service';
import {
    NgbTypeahead, NgbTypeaheadSelectItemEvent
} from '@ng-bootstrap/ng-bootstrap';
import { UiService } from '../../../common';

@Component({
    selector: 'app-dataservices-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.css'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class DetailComponent implements OnInit {

    public animation = '';
    public title = '';
    public editable = false;
    public model: DataServiceModel = { id: '', roles: [], tags: [], category: {} }; // eslint-disable-line max-len
    public dataSources: DataSourceModel[] = [];
    public dataSource?: DataSourceModel;

    public schemas: string[] = [];
    public tables: TableModel[] = [];
    public table?: TableModel;
    public columns: ColumnModel[] = [];

    // schema typeahead
    @ViewChild('schemaInstance', { static: false })
    public schemaInstance!: NgbTypeahead;
    public schemaFocus$ = new Subject<string>();
    public schemaClick$ = new Subject<string>();
    // table typeahead
    @ViewChild('tableInstance', { static: false })
    public tableInstance!: NgbTypeahead;
    public tableFocus$ = new Subject<string>();
    public tableClick$ = new Subject<string>();
    public supportMvt = false;
    public cacheSize = 0;

    private id = '';
    private reloadList = false;

    /* eslint-disable max-len */
    public searchSchema = (text$: Observable<string>): Observable<string[]> => {
        const debouncedText$ = text$.pipe(debounceTime(300), distinctUntilChanged());
        const clicksWithClosedPopup$ = this.schemaClick$.pipe(filter(() => !this.schemaInstance.isPopupOpen()));
        const inputFocus$ = this.schemaFocus$;
        return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
            map(term => term === '' ? this.schemas : this.schemas.filter(v => v.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
        );
    }
    public searchTable = (text$: Observable<string>): Observable<TableModel[]> => {
        const debouncedText$ = text$.pipe(debounceTime(300), distinctUntilChanged());
        const clicksWithClosedPopup$ = this.tableClick$.pipe(filter(() => !this.tableInstance.isPopupOpen()));
        const inputFocus$ = this.tableFocus$;
        return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
            map(term => term === '' ? this.tables.slice(0, 10) : this.tables.filter(v => v.name.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
        );
    }
    public tableFormatter = (t: TableModel | string): string => typeof t === 'string' ? t : t.name;
    /* eslint-enable: max-len */
    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private dataSourceService: DataSourceService,
        private ui: UiService,
        public meta: MetadataService,
        public account: AccountService,
        public vm: DataServiceService
    ) {
        const { id, editable } = route.snapshot.params;
        if (id === '0') {
            this.title = '新建数据服务';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑数据服务';
            this.editable = true;
        }
        else {
            this.title = '查看数据服务';
            this.editable = false;
        }
        this.id = id as string;
    }

    public async ngOnInit(): Promise<void> {
        this.dataSources = await this.dataSourceService.getAll();
        await this.vm.getAllRoles();
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                if (!model.roles) {
                    model.roles = [];
                }
                if (!model.tags) {
                    model.tags = [];
                }
                if (!model.category) {
                    model.category = {};
                }
                this.model = model;
                this.dataSource = this.dataSources.find(
                    ds => ds.id === model.dataSource?.id
                );
                this.table = { name: model.tableName as string };
                void this.loadSchemas()
                    .then(() => this.loadTables())
                    .then(() => this.loadColumns());
                this.supportMvt = await this.vm.supportMvt(this.id);
                if (this.supportMvt) {
                    void this.getMvtCacheSize();
                }
            }
        }
        // else {
        //     this.model.roles = Object.keys(roles);
        // }
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.fromState === '' && e.toState === 'void') {
            await this.router.navigate(['../'], { relativeTo: this.route });
            if (this.reloadList) {
                void this.vm.search();
            }
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

    public async save(): Promise<void> {
        if (!this.checkGeometryColumns()) {
            return;
        }
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.reloadList = true;
        this.goBack();
    }

    public getTableIcon(tableType: string): string {
        let icon = 'bi/';
        const type = tableType.toLowerCase();
        if (type.indexOf('view') > -1) {
            icon = icon + 'file-earmark-excel';
        }
        else {
            icon = icon + 'table';
        }
        return icon;
    }

    public async onSelectDataSource(): Promise<void> {
        this.model.dataSource = {
            id: this.dataSource?.id,
            name: this.dataSource?.name
        };
        if (this.dataSource?.databaseType !== 'mysql') {
            await this.loadSchemas();
        }
        else {
            await this.loadTables();
        }
    }

    public async onSelectSchema(e: NgbTypeaheadSelectItemEvent): Promise<void> {
        if (this.model.schema !== e.item as string) {
            this.model.schema = e.item as string;
        }
        await this.loadTables();
    }

    public async onSelectTable(e: NgbTypeaheadSelectItemEvent): Promise<void> {
        const table = e.item as TableModel;
        this.model.tableName = table.name;
        if (!this.model.name) {
            this.model.name = table.description || table.name;
        }
        await this.loadColumns();
    }

    public toggleField(name: string): void {
        if (!this.model.fields) {
            this.model.fields = [];
        }
        const idx = this.model.fields.findIndex(x => x.name === name);
        if (idx > -1) {
            this.model.fields.splice(idx, 1);
            return;
        }
        const col = this.columns.find(x => x.name === name);
        if (!col) {
            return;
        }
        const field: DataServiceFieldModel = {
            name: col.name,
            description: col.description,
            type: col.type,
            length: col.length,
            nullable: col.nullable,
            editable: false
        };
        this.model.fields.push(field);
    }

    public isFieldChecked(name: string): boolean {
        if (!this.model.fields) {
            return false;
        }
        return this.model.fields.findIndex(x => x.name === name) > -1;
    }

    public isAllFieldChecked(): boolean {
        if (!this.model.fields) {
            return false;
        }
        if (!this.columns) {
            return false;
        }
        return this.columns.length === this.model.fields.length;
    }

    public toggleAllField(): void {
        if (this.columns.length === 0) {
            return;
        }
        if (!this.model.fields) {
            this.model.fields = [];
        }
        if (this.isAllFieldChecked()) {
            this.model.fields = [];
            return;
        }
        this.model.fields = [];
        this.columns.forEach(col => {
            this.toggleField(col.name);
         });
    }

    public toggleFieldEditable(name: string): void {
        if (!this.model.fields) {
            return;
        }
        const idx = this.model.fields.findIndex(x => x.name === name);
        if (idx === -1) {
            return;
        }
        this.model.fields[idx].editable = !this.model.fields[idx].editable;
    }

    public isFieldEditable(name: string): boolean {
        if (!this.model.fields) {
            return false;
        }
        const field = this.model.fields.find(x => x.name === name);
        return field?.editable as boolean;
    }

    public isFieldsValid(): boolean {
        return !!this.model.primaryKeyColumn &&
            !!this.model.displayColumn &&
            !!this.model.fields &&
            this.model.fields.length > 0;
    }

    public hasGeoColumn(): boolean {
        return  !!(this.model.fields?.find(x => x.type === 'geometry' || x.type?.startsWith('geometry(')));
    }

    public async deleteMvtCache(): Promise<void> {
        if (!this.supportMvt) {
            return;
        }
        const deleted = await this.vm.deleteMvtCache(this.id);
        if (deleted) {
            await this.getMvtCacheSize();
        }
    }

    private async getMvtCacheSize(): Promise<void> {
        this.cacheSize = await this.vm.getMvtCache(this.id)
    }

    private async loadSchemas(): Promise<void> {
        if (!this.dataSource) {
            return;
        }
        const schemas = await this.meta.getSchemas(
            this.dataSource.id as string
        );
        this.schemas = schemas;
    }

    private async loadTables(): Promise<void> {
        if (!this.dataSource) {
            return;
        }
        let schema = '';
        if (this.dataSource.databaseType !== 'mysql') {
            schema = this.model.schema as string;
        }
        const tables = await this.meta.getTables(
            this.dataSource.id as string,
            schema
        );
        this.tables = tables;
    }

    private async loadColumns(): Promise<void> {
        if (!this.dataSource) {
            return;
        }
        if (!this.model.tableName) {
            return;
        }
        const columns = await this.meta.getColumns(
            this.dataSource.id as string,
            this.model.schema as string,
            this.model.tableName as string
        );
        this.columns = columns;
    }

    private checkGeometryColumns(): boolean {
        const geometryCols = this.model.fields?.filter(
            x => x.type === 'geometry' || x.type?.startsWith('geometry(')
        );
        if (!!geometryCols) {
            if (geometryCols.length > 1) {
                this.ui.showAlert({
                    type: 'danger',
                    message: '一个数据源只能包含一个空间数据列， 请重新选择！'
                });
                return false;
            }
            else {
                if (geometryCols.length > 0) {
                    this.model.geometryColumn = geometryCols[0].name;
                }
                else {
                    delete this.model.geometryColumn;
                }
            }
        }
        else {
            if (!!this.model.geometryColumn) {
                delete this.model.geometryColumn;
            }
        }
        return true;
    }

}
