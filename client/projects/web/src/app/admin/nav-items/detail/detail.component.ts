import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';
import {
    NavItemsService, NavItemModel, MenuOption
} from '../nav-items.service';
import { ServerFolderBrowserComponent } from '../../../common';

@Component({
    selector: 'app-nav-item-detail',
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
    public formTitle: string;
    public editable: boolean;
    public targets = [
        { name: '当前窗口', value: '' },
        { name: '内嵌窗口', value: '_iframe' }
    ];
    public model: NavItemModel = { id: '', target: '', roles: [] };
    public selectedRoles: string[] = [];
    public parents: MenuOption[] = [];

    private id: string;
    private reloadList = false;

    constructor(
        private router: Router,
        private modal: NgbModal,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: NavItemsService
    ) {
        const id = route.snapshot.params.id;
        const editable = route.snapshot.params.editable;
        if (id === '0') {
            this.formTitle = '新建菜单项';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.formTitle = '编辑菜单项';
            this.editable = true;
        }
        else {
            this.formTitle = '查看菜单项';
            this.editable = false;
        }
        this.id = id;
    }

    public async ngOnInit(): Promise<void> {
        await this.vm.getAllRoles();
        this.parents = await this.vm.getMenuOptions();
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                this.model = model;
            }
            this.selectedRoles = this.model.roles ?? [];
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
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.reloadList = true;
        this.goBack();
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

    public showIconDialog(): void {
        const modalRef = this.modal.open(
            ServerFolderBrowserComponent,
            { size: 'lg', scrollable: true }
        );
        modalRef.componentInstance.title = '选择图标';
        modalRef.componentInstance.params = {
            alias: 'icons',
            path: '.',
            filter: '*.*',
            files: [],
            folders: []
        };
    }

}
