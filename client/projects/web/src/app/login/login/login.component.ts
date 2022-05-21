import { Component, ErrorHandler } from '@angular/core';
import { NgForm } from '@angular/forms';

import { AccountService, LoginModel } from 'app-shared';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {

    public model: LoginModel = {};
    public loading = false;
    public message = new Subject<string | undefined>();

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private acntSvc: AccountService,
        private errorHandler: ErrorHandler
    ) { }

    public async login(): Promise<void> {
        try {
            this.loading = true;
            await this.acntSvc.login(this.model);
            await this.acntSvc.getInfo();
            let { returnUrl } = this.route.snapshot.params;
            if (!returnUrl) {
                returnUrl = 'home';
            }
            await this.router.navigate(
                [`/${returnUrl}`],
                { replaceUrl: true }
            );
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            const message = typeof ex.error === 'string' ? ex.error : '无法登录！'; // eslint-disable-line max-len, @typescript-eslint/no-unsafe-member-access, @typescript-eslint/no-unsafe-assignment
            this.message.next(message);
        }
        finally {
            this.loading = false;
        }
    }

    public clearMessage(): void {
        this.message.next(undefined);
    }

    public passwordKeyUp(e: KeyboardEvent, loginForm: NgForm): void {
        if (e.key === 'Enter' && loginForm.valid) {
            void this.login();
        }
    }

}
