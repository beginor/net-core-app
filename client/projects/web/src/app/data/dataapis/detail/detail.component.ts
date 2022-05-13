import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';

import { DataApiService, DataApiModel } from '../dataapis.service';
import {
    DataSourceService, DataSourceModel
} from '../../datasources/datasources.service';

@Component({
    selector: 'app-dataapi-detail',
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
    public model: DataApiModel = { id: '', roles: [], tags: [], category: {} };
    public dataSources: DataSourceModel[] = [];
    public dataSource?: DataSourceModel;

    private id = '';
    private reloadList = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private dataSourceService: DataSourceService,
        public account: AccountService,
        public vm: DataApiService
    ) {
        const { id, editable } = route.snapshot.params;
        if (id === '0') {
            this.title = '新建数据接口';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑数据接口';
            this.editable = true;
        }
        else {
            this.title = '查看数据接口';
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
            }
        }
        else {
            this.model.roles = [];
            this.model.statement = [
                '<Statement Id="">',
                '  <!-- 动态 SQL 标签请参考 https://smartsql.net/config/sqlmap.html#statement-筛选子标签 -->', // eslint-disable-line max-len
                '</Statement>'
            ].join('\n');
            this.model.columns = [];
            this.model.parameters = [];
        }
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
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.reloadList = true;
        this.goBack();
    }

    public onSelectDataSource(): void {
        this.model.dataSource = {
            id: this.dataSource?.id,
            name: this.dataSource?.name
        };
    }

    public isRoleChecked(role: string): boolean {
        if (!this.model.roles) {
            return false;
        }
        return this.model.roles.indexOf(role) > -1;
    }

    public toggleCheckedRole(role: string): void {
        if (!this.model.roles) {
            return;
        }
        const idx = this.model.roles.indexOf(role);
        if (idx > -1) {
            this.model.roles.splice(idx, 1);
        }
        else {
            this.model.roles.push(role);
        }
    }

}
