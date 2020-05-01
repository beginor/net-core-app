import { Component, OnInit, ErrorHandler } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AccountService, LoginModel } from 'app-shared';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

    public model: LoginModel = { };
    public loading = false;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        private accountSvc: AccountService,
        private errorHandler: ErrorHandler
    ) { }

    public ngOnInit(): void {
    }

    public async login(): Promise<void> {
        if (this.loading) {
            return;
        }
        try {
            this.loading = true;
            await this.accountSvc.login(this.model);
            await this.accountSvc.getInfo();
            const returnUrl = this.route.snapshot.params.returnUrl || 'home';
            this.router.navigate(
                ['/' + returnUrl],
                { replaceUrl: true }
            );
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.snackBar.open(ex.error, '确定', { duration: 3000 });
        }
        finally {
            this.loading = false;
        }
    }

}
