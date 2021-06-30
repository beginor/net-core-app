import { Inject, Injectable } from '@angular/core';
import {
    HttpInterceptor, HttpEvent, HttpRequest, HttpHandler
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from './account.service';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {

    constructor(
        @Inject('apiRoot') private apiRoot: string,
        private account: AccountService
    ) { }

    public intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        if (req.url.startsWith(this.apiRoot)) {
            const setHeaders: { [key: string]: string } = {
                'X-Requested-With': 'XMLHttpRequest'
            };
            if (!!this.account.token) {
                setHeaders['Authorization'] = `Bearer ${this.account.token}`;
            }
            req = req.clone({
                // withCredentials: true
                setHeaders
            });
        }
        return next.handle(req);
    }

}
