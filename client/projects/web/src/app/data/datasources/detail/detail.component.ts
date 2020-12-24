import { Component, OnInit, ViewChild } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';
import { Observable, Subject, merge } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, map } from 'rxjs/operators';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';

import { DataSourceService, DataSourceModel } from '../datasources.service';
import { MetadataService, TableModel, ColumnModel } from '../metadata.service';
import {
    ConnectionService, ConnectionModel
} from '../../connections/connections.service';
import { NgbTypeahead, NgbTypeaheadConfig, NgbTypeaheadSelectItemEvent } from '@ng-bootstrap/ng-bootstrap';

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
    public schema?: string;
    public tables: TableModel[] = [];
    public tableName?: string;
    public columns: ColumnModel[] = [];

    // schema typeahead
    @ViewChild('schemaInstance', { static: false })
    public schemaInstance!: NgbTypeahead;
    public schemaFocus$ = new Subject<string>();
    public schemaClick$ = new Subject<string>();
    public searchSchema = (text$: Observable<string>) => {
        const debouncedText$ = text$.pipe(debounceTime(300), distinctUntilChanged());
        const clicksWithClosedPopup$ = this.schemaClick$.pipe(filter(() => !this.schemaInstance.isPopupOpen()));
        const inputFocus$ = this.schemaFocus$;
        return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
            map(term => term === '' ? this.schemas : this.schemas.filter(v => v.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
        );
    }
    // table typeahead
    @ViewChild('tableInstance', { static: true })
    public tableInstance!: NgbTypeahead;
    public tableFocus$ = new Subject<string>();
    public tableClick$ = new Subject<string>();
    public searchTable = (text$: Observable<string>) => {
        const debouncedText$ = text$.pipe(debounceTime(300), distinctUntilChanged());
        const clicksWithClosedPopup$ = this.tableClick$.pipe(filter(() => !this.schemaInstance.isPopupOpen()));
        const inputFocus$ = this.tableFocus$;
        return merge(debouncedText$, inputFocus$, clicksWithClosedPopup$).pipe(
            map(term => term === '' ? this.tables.slice(1, 10) : this.tables.filter(v => v.tableName.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
        );
    };
    public tableFormatter = (t: TableModel) => t.tableName;

    private id = '';
    private reloadList = false;

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
                this.onChangeConnection();
            }
        }
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.phaseName === 'done' && e.toState === 'void') {
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
        if (this.connection) {
            this.model.connection = { id: this.connection.id }
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

    public async onChangeConnection(): Promise<void> {
        if (!this.connection) {
            return;
        }
        if (this.connection.databaseType === 'mysql') {
            this.loadTables('');
        }
        else {
            const schemas = await this.meta.getSchemas(this.connection.id as string);
            this.schemas = schemas;
        }
    }

    public async onChangeSchema($event: NgbTypeaheadSelectItemEvent<string>): Promise<void> {
        await this.loadTables($event.item);
    }

    private async loadTables(schema: string): Promise<void> {
        if (!this.connection) {
            return;
        }
        const tables = await this.meta.getTables(
            this.connection.id as string,
            schema
        );
        this.tables = tables;
    }

}
