import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight } from 'app-shared';
import { RolesService } from '../roles.service';

@Component({
    selector: 'app-role-privilege',
    templateUrl: './privilege.component.html',
    styleUrls: ['./privilege.component.css'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class PrivilegeComponent implements OnInit {

    public animation = '';
    public title: string;

    private id: string;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public vm: RolesService
    ) {
        const { id, desc } = route.snapshot.params;
        this.id = id;
        this.title = `${desc}权限列表`;
    }

    public ngOnInit(): void {
        this.vm.getPrivilegesForRole(this.id);
        this.vm.getAllPrivileges();
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.fromState === '' && e.toState === 'void') {
            await this.router.navigate(['../../'], { relativeTo: this.route });
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

    public async togglePrivilege(e: Event): Promise<void> {
        e.preventDefault();
        e.stopPropagation();
        const checkbox = e.target as HTMLInputElement;
        const privilege = checkbox.value;
        await this.vm.toggleRolePrivilege(this.id, privilege);
    }

}
