import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight, AccountService } from 'services';
import { AppPrivilegeModel, AppPrivilegeService } from '../privileges.service';

@Component({
    selector: 'app-privilege-detail',
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
    public title: string;
    public editable: boolean;
    public model: AppPrivilegeModel = {};

    private id: string;
    private reloadList = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: AppPrivilegeService
    ) {
        const id = route.snapshot.params.id;
        const editable = route.snapshot.params.editable;
        if (id === '0') {
            this.title = '新建系统权限';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑系统权限';
            this.editable = true;
        }
        else {
            this.title = '查看系统权限';
            this.editable = false;
        }
        this.id = id;
    }

    public async ngOnInit(): Promise<void> {
        if (this.id !== '0') {
            this.model = await this.vm.getById(this.id);
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
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.reloadList = true;
        this.goBack();
    }

}
