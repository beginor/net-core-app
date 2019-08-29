import { Inject } from '@angular/core';
import {
    HttpInterceptor, HttpEvent, HttpRequest, HttpHandler
} from '@angular/common/http';
import { Observable } from 'rxjs';

export class ApiInterceptor implements HttpInterceptor {

    constructor(
        @Inject('apiRoot') private apiRoot: string
    ) { }

    public intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        if (req.url.startsWith(this.apiRoot)) {
            const request = req.clone({
                withCredentials: true,
                setHeaders: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });
            return next.handle(request);
        }
        return next.handle(req);
    }

}
