import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight } from '../../../common';
import { AppPrivilegeModel } from '../privileges.service';

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

    constructor(
        private router: Router,
        private route: ActivatedRoute,
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
    }

    public ngOnInit(): void {
    }

    public onAnimationEvent(e: AnimationEvent): void {
        if (e.phaseName === 'done' && e.toState === 'void') {
            this.router.navigate(['../'], { relativeTo: this.route });
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

}
