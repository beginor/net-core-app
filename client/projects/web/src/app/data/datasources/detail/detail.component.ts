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

import { DataSourceService, DataSourceModel } from '../datasources.service';
import { MetadataService, TableModel, ColumnModel } from '../metadata.service';
import {
    ConnectionService, ConnectionModel
} from '../../connections/connections.service';
import { NgbTypeahead, NgbTypeaheadSelectItemEvent } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-datasource-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.scss'],
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
    public model: DataSourceModel = {};
    public connections: ConnectionModel[] = [];
    public connection?: ConnectionModel;

    public schemas: string[] = [];
    public tables: TableModel[] = [];
    public table?: TableModel;
    public columns: ColumnModel[] = [];
    public pkColumn?: ColumnModel;
    public displayColumn?: ColumnModel;
    public geoColumn?: ColumnModel;

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
    // pk col typeahead
    @ViewChild('pkColInstance', { static: false })
    public pkColInstance!: NgbTypeahead;
    public pkColFocus$ = new Subject<string>();
    public pkColClick$ = new Subject<string>();
    // display col typeahead
    @ViewChild('displayColInstance', { static: false })
    public displayColInstance!: NgbTypeahead;
    public displayColFocus$ = new Subject<string>();
    public displayColClick$ = new Subject<string>();
    @ViewChild('geoColInstance', { static: false })
    public geoColInstance!: NgbTypeahead;
    public geoColFocus$ = new Subject<string>();
    public geoColClick$ = new Subject<string>();

    private id = '';
    private reloadList = false;

    // tslint:disable: max-line-length
    public searchSchema = (text$: Observable<string>) => {
        const debouncedText$ = text$.pipe(debounceTime(300), distinctUntilChanged());
        const clicksWithClosedPopup$ = this.schemaClick$.pipe(filter(() => !this.schemaInstance.isPopupOpen()));
        const inputFocus$ = this.schemaFocus$;
        return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
            map(term => term === '' ? this.schemas : this.schemas.filter(v => v.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
        );
    }
    public searchTable = (text$: Observable<string>) => {
        const debouncedText$ = text$.pipe(debounceTime(300), distinctUntilChanged());
        const clicksWithClosedPopup$ = this.tableClick$.pipe(filter(() => !this.tableInstance.isPopupOpen()));
        const inputFocus$ = this.tableFocus$;
        return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
            map(term => term === '' ? this.tables.slice(0, 10) : this.tables.filter(v => v.tableName.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
        );
    }
    public searchPkColumns = (text$: Observable<string>) => {
        const debouncedText$ = text$.pipe(debounceTime(300), distinctUntilChanged());
        const clicksWithClosedPopup$ = this.pkColClick$.pipe(filter(() => !this.pkColInstance.isPopupOpen()));
        const inputFocus$ = this.pkColFocus$;
        return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
            map(term => term === '' ? this.columns.slice(0, 10) : this.columns.filter(c => c.columnName.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
        );
    }
    public searchDisplayColumns = (text$: Observable<string>) => {
        const debouncedText$ = text$.pipe(debounceTime(300), distinctUntilChanged());
        const clicksWithClosedPopup$ = this.displayColClick$.pipe(filter(() => !this.displayColInstance.isPopupOpen()));
        const inputFocus$ = this.displayColFocus$;
        return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
            map(term => term === '' ? this.columns.slice(0, 10) : this.columns.filter(c => c.columnName.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
        );
    }
    public searchGeoColumns = (text$: Observable<string>) => {
        const debouncedText$ = text$.pipe(debounceTime(300), distinctUntilChanged());
        const clicksWithClosedPopup$ = this.geoColClick$.pipe(filter(() => !this.geoColInstance.isPopupOpen()));
        const inputFocus$ = this.geoColFocus$;
        return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
            map(term => term === '' ? this.columns.slice(0, 10) : this.columns.filter(c => c.columnName.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
        );
    }
    public tableFormatter = (t: TableModel | string) => typeof t === 'string' ? t : t.tableName;
    public colFormater = (c: ColumnModel | string) => typeof c === 'string' ? c : c.columnName;
    // tslint:enable: max-line-length

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private conn: ConnectionService,
        public meta: MetadataService,
        public account: AccountService,
        public vm: DataSourceService
    ) {
        const id = route.snapshot.params.id;
        const editable = route.snapshot.params.editable;
        if (id === '0') {
            this.title = '新建数据源';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑数据源';
            this.editable = true;
        }
        else {
            this.title = '查看数据源';
            this.editable = false;
        }
        this.id = id;
    }

    public async ngOnInit(): Promise<void> {
        this.connections = await this.conn.getAll();
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                this.model = model;
                this.connection = this.connections.find(
                    cs => cs.id === model.connection?.id
                );
                this.table = { tableName: model.tableName as string };
                this.pkColumn = {
                    columnName: model.primaryKeyColumn as string
                };
                this.displayColumn = {
                    columnName: model.displayColumn as string
                };
                this.geoColumn = {
                    columnName: model.geometryColumn as string
                };
                this.loadSchemas()
                    .then(() => this.loadTables())
                    .then(() => this.loadColumns());
            }
        }
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.fromState === '' && e.toState === 'void') {
            await this.router.navigate(['../'], { relativeTo: this.route });
            if (this.reloadList) {
                this.vm.search();
            }
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

    public async save(): Promise<void> {
        if (!this.geoColumn) {
            delete this.model.geometryColumn;
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

    public async onSelectConnection(): Promise<void> {
        this.model.connection = {
            id: this.connection?.id,
            name: this.connection?.name
        };
        if (this.connection?.databaseType !== 'mysql') {
            await this.loadSchemas();
        }
        else {
            await this.loadTables();
        }
    }

    public async onSelectSchema(e: NgbTypeaheadSelectItemEvent): Promise<void> {
        if (this.model.schema !== e.item as string) {
            this.model.schema = e.item;
        }
        await this.loadTables();
    }

    public async onSelectTable(e: NgbTypeaheadSelectItemEvent): Promise<void> {
        const table = e.item as TableModel;
        this.model.tableName = table.tableName;
        if (!this.model.name) {
            this.model.name = table.description || table.tableName;
        }
        await this.loadColumns();
    }

    public onSelectPkColumn(e: NgbTypeaheadSelectItemEvent): void {
        this.model.primaryKeyColumn = e.item.columnName;
    }

    public onSelectDisplayColumn(e: NgbTypeaheadSelectItemEvent): void {
        this.model.displayColumn = e.item.columnName;
    }

    public onSelectGeoColumn(e: NgbTypeaheadSelectItemEvent): void {
        this.model.geometryColumn = e.item.columnName;
    }

    private async loadSchemas(): Promise<void> {
        if (!this.connection) {
            return;
        }
        const schemas = await this.meta.getSchemas(
            this.connection.id as string
        );
        this.schemas = schemas;
    }

    private async loadTables(): Promise<void> {
        if (!this.connection) {
            return;
        }
        let schema = '';
        if (this.connection.databaseType !== 'mysql') {
            schema = this.model.schema as string;
        }
        const tables = await this.meta.getTables(
            this.connection.id as string,
            schema
        );
        this.tables = tables;
    }

    private async loadColumns(): Promise<void> {
        if (!this.connection) {
            return;
        }
        if (!this.model.tableName) {
            return;
        }
        const columns = await this.meta.getColumns(
            this.connection.id as string,
            this.model.schema as string,
            this.model.tableName as string
        );
        this.columns = columns;
    }

}
