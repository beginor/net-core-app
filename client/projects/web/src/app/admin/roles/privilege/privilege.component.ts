import { Component, OnInit } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight } from 'services';

@Component({
    selector: 'app-role-privilege',
    templateUrl: './privilege.component.html',
    styleUrls: ['./privilege.component.scss'],
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
        private route: ActivatedRoute
    ) {
        const id = route.snapshot.params.id;
        const desc = route.snapshot.params.desc;
        this.id = id;
        this.title = `${desc}权限列表`;
    }

    public ngOnInit(): void {
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.phaseName === 'done' && e.toState === 'void') {
            await this.router.navigate(['../../'], { relativeTo: this.route });
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

}
