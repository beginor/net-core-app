import { Component } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import {
    NgbCalendar, NgbDateStruct, NgbDateParserFormatter
} from '@ng-bootstrap/ng-bootstrap';

import { slideInRight, slideOutRight, AccountService } from 'app-shared';
import { UsersService } from '../users.service';

@Component({
    selector: 'app-user-lock',
    templateUrl: './lock.component.html',
    styleUrls: ['./lock.component.scss'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class LockComponent {

    public animation = '';
    public title: string;
    public editable: boolean;

    public lockForm: FormGroup;

    private userId: string;

    public get lockoutEnd(): FormControl {
        return this.lockForm.get('lockoutEnd') as FormControl;
    }

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private dateFormatter: NgbDateParserFormatter,
        public calendar: NgbCalendar,
        public account: AccountService,
        public vm: UsersService
    ) {
        const fullname = route.snapshot.params.fullname || '用户';
        this.title = `锁定 ${fullname}`;
        this.editable = true;
        this.userId = route.snapshot.params.id;
        //
        const nextDay = calendar.getNext(calendar.getToday(), 'd', 1);
        this.lockForm  = new FormGroup({
            lockoutEnd: new FormControl(
                { value: nextDay, disabled: !this.editable },
                Validators.required
            )
        });
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

    public save(): void {
        const value = this.lockoutEnd.value as NgbDateStruct;
        const date = this.dateFormatter.format(value);
        this.vm.lockUser(this.userId, `${date} 23:59:59`).then(
            () => this.goBack()
        );
    }

}
