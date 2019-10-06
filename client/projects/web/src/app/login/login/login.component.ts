import { Component, OnInit } from '@angular/core';

import { AccountService, LoginModel } from 'services';
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
    public message = new Subject<string>();

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private acntSvc: AccountService
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
            console.error(ex);
            const message = typeof ex.error === 'string' ? ex.error : '无法登录！';
            this.message.next(message);
        }
        finally {
            this.loading = false;
        }
    }

    public clearMessage(): void {
        this.message.next(null);
    }

}
