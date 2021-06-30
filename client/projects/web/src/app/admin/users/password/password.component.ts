import { Component } from '@angular/core';
import {
    trigger, transition, useAnimation, AnimationEvent
} from '@angular/animations';
import {
    FormGroup, FormControl, Validators
} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import {
    slideInRight, slideOutRight, AccountService, confirmTo
} from 'app-shared';
import { UsersService } from '../users.service';

@Component({
    selector: 'app-user-password',
    templateUrl: './password.component.html',
    styleUrls: ['./password.component.scss'],
    animations: [
        trigger('animation', [
            transition(':enter', useAnimation(slideInRight)),
            transition(':leave', useAnimation(slideOutRight))
        ])
    ]
})
export class PasswordComponent {

    public animation = '';
    public title: string;
    public editable: boolean;
    public form: FormGroup;

    public get password(): FormControl {
        return this.form.get('password') as FormControl;
    }

    public get confirmPassword(): FormControl {
        return this.form.get('confirmPassword') as FormControl;
    }

    private userId: string;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: UsersService
    ) {
        this.userId = route.snapshot.params.id;
        const fullname = route.snapshot.params.fullname || '用户';
        this.title = `重置 ${fullname} 的密码`;
        this.editable = true;
        this.form = new FormGroup({
            password: new FormControl(
                { value: '', disabled: !this.editable },
                [ Validators.required, Validators.minLength(8) ]
            ),
            confirmPassword: new FormControl(
                { value: '', disabled: !this.editable },
                [ Validators.required, confirmTo('password') ]
            )
        });
    }

    public async onAnimationEvent(e: AnimationEvent): Promise<void> {
        if (e.fromState === '' && e.toState === 'void') {
            await this.router.navigate(['../..'], { relativeTo: this.route });
        }
    }

    public goBack(): void {
        this.animation = 'void';
    }

    public async save(): Promise<void> {
        await this.vm.resetPass(this.userId, this.form.value);
        this.goBack();
    }

}
