import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';
import { UserModel, UsersService} from '../users.service';

@Component({
    selector: 'app-users-detail',
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
    public title: string;
    public editable: boolean;
    public model: UserModel = { id: '', lockoutEnabled: true, gender: '保密' };
    public dob = { year: 1970, month: 1, day: 1 };

    private id: string;
    private reloadList = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: UsersService
    ) {
        const { id, editable } = route.snapshot.params;
        if (id === '0') {
            this.title = '新建用户';
            this.editable = true;
        }
        else if (editable === 'true') {
            this.title = '编辑用户';
            this.editable = true;
        }
        else {
            this.title = '查看用户';
            this.editable = false;
        }
        this.id = id as string;
    }

    public async ngOnInit(): Promise<void> {
        if (this.id !== '0') {
            const model = await this.vm.getById(this.id);
            if (!!model) {
                this.model = model;
            }
            if (!!this.model.dateOfBirth) {
                const dob = this.model.dateOfBirth.split('-');
                this.dob = {
                    year: parseInt(dob[0], 10),
                    month: parseInt(dob[1], 10),
                    day: parseInt(dob[2], 10)
                };
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
        const dob = `${this.dob.year}-${this.dob.month}-${this.dob.day}`;
        this.model.dateOfBirth = dob;
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
