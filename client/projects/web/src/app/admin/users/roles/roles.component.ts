import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';
import { UserModel, UsersService } from '../users.service';

@Component({
    selector: 'app-roles',
    templateUrl: './roles.component.html',
    styleUrls: ['./roles.component.scss'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class RolesComponent implements OnInit {

    public animation = '';
    public title: string;
    public editable: boolean;

    private userId: string;
    private userRoles: { [key: string]: boolean } = {};

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: UsersService
    ) {
        this.userId = route.snapshot.params.id;
        const fullname = route.snapshot.params.fullname || '用户';
        this.title = `设置 ${fullname} 的角色`;
        this.editable = true;
    }

    public async ngOnInit(): Promise<void> {
        await this.vm.getRoles();
        const roles = await this.vm.getUserRoles(this.userId);
        for (const role of roles) {
            this.userRoles[role] = true;
        }
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.fromState === '' && e.toState === 'void') {
            await this.router.navigate(['../..'], { relativeTo: this.route });
            // if (this.reloadList) {
            //     this.vm.search();
            // }
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

    public async save(): Promise<void> {
        const toDelete = [];
        const toAdd = [];
        for (const role in this.userRoles) {
            if (this.userRoles.hasOwnProperty(role)) {
                const isAdd = this.userRoles[role];
                if (isAdd) {
                    toAdd.push(role);
                }
                else {
                    toDelete.push(role);
                }
            }
        }
        await this.vm.saveUserRoles(this.userId, toAdd, toDelete);
        this.goBack();
    }

    public isChecked(roleName: string): boolean {
        return !!this.userRoles[roleName];
    }

    public toggleUserRole($event: Event, roleName: string): void {
        const target = $event.target as HTMLInputElement;
        this.userRoles[roleName] = target.checked;
    }

}
