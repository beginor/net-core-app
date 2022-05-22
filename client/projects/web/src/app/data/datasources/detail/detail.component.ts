import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';

import {
    DataSourceService, DataSourceModel, StatusResult
} from '../datasources.service';

@Component({
    selector: 'app-datasources-detail',
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
    public model: DataSourceModel = { id: '', timeout: 10 };
    public showPass = false;
    public checkingStatus = false;
    public showCheckButton = true;
    public showCheckResult = false;
    public checkResult!: StatusResult;

    private id = '';
    private reloadList = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: DataSourceService
    ) {
        const { id, editable } = route.snapshot.params;
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
        this.id = id as string;
        this.resetStatusResult();
    }

    public async ngOnInit(): Promise<void> {
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                this.model = model;
            }
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

    public canCheckStatus(): boolean {
        const canCheck = this.showCheckButton && (!!this.model.databaseType) &&
            (!!this.model.serverAddress) &&
            (!!this.model.serverPort) &&
            (!!this.model.databaseName) &&
            (!!this.model.username) &&
            (!!this.model.password) &&
            (!!this.model.timeout);
        return canCheck;
    }

    public async checkStatus(): Promise<void> {
        this.showCheckButton = false;
        this.showCheckResult = true;
        this.resetStatusResult();
        this.checkingStatus = true;
        const result = await this.vm.checkStatus(this.model);
        this.checkingStatus = false;
        this.checkResult = result;
    }

    public onCheckAlertClosed(): void {
        this.showCheckButton = true;
        this.showCheckResult = false;
        this.resetStatusResult();
    }

    private resetStatusResult(): void {
        this.checkResult = { status: 'info', message: '测试中 ...' };
    }

}
