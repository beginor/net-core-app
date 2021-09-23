import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';
import {
    NgbCalendar, NgbDateParserFormatter, NgbDateStruct, NgbDate
} from '@ng-bootstrap/ng-bootstrap';

import {
    slideInRight, slideOutRight, AccountService, UserTokenModel, AppRole,
    AppPrivilege
} from 'app-shared';
import { TokenService } from '../token.service';
import { UiService } from '../../common';


@Component({
    selector: 'app-token-detail',
    templateUrl: './token-detail.component.html',
    styleUrls: ['./token-detail.component.scss'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class TokenDetailComponent implements OnInit {

    public animation = '';
    public title = '';
    public model: UserTokenModel = { id: '', name: '', value: '' };
    public roles: AppRole[] = [];
    public privileges: Array<{ module: string; privileges: AppPrivilege[] }> = []; // eslint-disable-line max-len
    public tokenExpiresAt: NgbDateStruct | null = null;
    public tokenUrls: string[] = [];
    private id = '';
    private reloadList = false;
    private checkedRoles: string[] = [];
    private checkedPrivileges: string[] = [];

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private account: AccountService,
        private ui: UiService,
        private dateFormatter: NgbDateParserFormatter,
        public calendar: NgbCalendar,
        public vm: TokenService
    ) {
        const { id } = route.snapshot.params;
        this.title = id === '0' ? '新建凭证' : '凭证信息';
        this.id = id as string;
    }

    public async ngOnInit(): Promise<void> {
        const rap = await  this.account.getRolesAndPrivileges();
        this.roles = rap.roles;
        this.privileges = [];
        for (const p of rap.privileges) {
            let mp = this.privileges.find(
                m => m.module == p.module
            );
            if (!mp) {
                mp = { module: p.module ?? '', privileges: [] };
                this.privileges.push(mp);
            }
            mp.privileges.push(p);
        }
        if (this.id !== '0') {
            const model = this.vm.getById(this.id);
            if (!!model) {
                this.model = model;
                if (!!model.roles && model.roles.length > 0) {
                    this.checkedRoles = JSON.parse(
                        JSON.stringify(model.roles)
                    ) as string[];
                }
                if (!!model.privileges && model.privileges.length > 0) {
                    this.checkedPrivileges = JSON.parse(
                        JSON.stringify(model.privileges)
                    ) as string[];
                }
                if (!!model.expiresAt) {
                    this.tokenExpiresAt = this.dateFormatter.parse(
                        model.expiresAt
                    );
                }
                if (!!model.urls && model.urls.length > 0) {
                    this.tokenUrls = JSON.parse(
                        JSON.stringify(model.urls)
                    ) as string[];
                }
            }
        }
        else {
            void this.newTokenValue();
        }
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.fromState === '' && e.toState === 'void') {
            await this.router.navigate(['../'], { relativeTo: this.route });
            if (this.reloadList) {
                void this.vm.loadTokens();
            }
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

    public async save(): Promise<void> {
        this.model.roles = this.checkedRoles;
        this.model.privileges = this.checkedPrivileges;
        if (!!this.tokenExpiresAt) {
            this.model.expiresAt = this.dateFormatter.format(
                this.tokenExpiresAt
            );
        }
        this.model.urls = this.tokenUrls;
        if (this.id !== '0') {
            await this.vm.update(this.id, this.model);
        }
        else {
            await this.vm.create(this.model);
        }
        this.reloadList = true;
        this.goBack();
    }

    public async newTokenValue(): Promise<void> {
        try {
            const val = await this.account.newTokenValue();
            if (!!val) {
                this.model.value = val;
            }
        }
        catch (ex: unknown) {
            this.ui.showAlert(
                { type: 'danger', message: '生成新凭证值出错！' }
            );
        }
    }

    public isChecked(roleName: string, propName: ArrPropName): boolean {
        const arr = this[propName];
        if (!arr) {
            return false;
        }
        return arr.indexOf(roleName) > -1;
    }

    public toggleChecked(
        $event: Event,
        roleName: string,
        propName: ArrPropName
    ): void {
        if (!this[propName]) {
            this[propName] = [];
        }
        const arr = this[propName];
        const target = $event.target as HTMLInputElement;
        if (target.checked) {
            arr.push(roleName);
        }
        else {
            const idx = arr.indexOf(roleName);
            arr.splice(idx, 1);
        }
    }

    public addTokenUrl(el: HTMLInputElement): void {
        const url = el.value.trim();
        if (!url) {
            return;
        }
        const idx = this.tokenUrls.indexOf(url);
        if (idx < 0) {
            this.tokenUrls.push(url);
        }
        el.value = '';
    }

    public removeTokenUrl(url: string): void {
        const idx = this.tokenUrls.indexOf(url);
        if (idx > -1) {
            this.tokenUrls.splice(idx, 1);
        }
    }

}

export type ArrPropName = 'checkedRoles' | 'checkedPrivileges';
