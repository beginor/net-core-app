import {Component, OnInit} from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { Router, ActivatedRoute } from '@angular/router';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';

import { DataApiService, DataApiModel } from '../dataapis.service';

@Component({
    selector: 'app-statement',
    templateUrl: './statement.component.html',
    styleUrls: ['./statement.component.scss'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class StatementComponent implements OnInit {

    public animation = '';
    public title = '';
    public model: DataApiModel = { id: '' };

    private id: string;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public vm: DataApiService
    ) {
        const { id } = route.snapshot.params;
        this.id = id;
    }

    public ngOnInit(): void {
    }

}
