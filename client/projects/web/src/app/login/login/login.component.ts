import { Component, OnInit, ErrorHandler } from '@angular/core';
import { NgForm } from '@angular/forms';

import { AccountService, LoginModel } from 'app-shared';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

    public model: LoginModel = {};
    public loading = false;
    public message = new Subject<string | undefined>();

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private acntSvc: AccountService,
        private errorHandler: ErrorHandler
    ) { }

    public ngOnInit(): void {
    }

    public async login(): Promise<void> {
        try {
            this.loading = true;
            await this.acntSvc.login(this.model);
            await this.acntSvc.getInfo();
            const returnUrl = this.route.snapshot.params.returnUrl || 'home';
            await this.router.navigate(
                ['/' + returnUrl],
                { replaceUrl: true }
            );
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            const message = typeof ex.error === 'string' ? ex.error : '无法登录！';
            this.message.next(message);
        }
        finally {
            this.loading = false;
        }
    }

    public clearMessage(): void {
        this.message.next();
    }

    public passwordKeyUp(e: KeyboardEvent, loginForm: NgForm): void {
        if (e.key === 'Enter' && loginForm.valid) {
            this.login();
        }
    }

}
