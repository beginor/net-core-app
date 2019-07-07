import { Inject } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor } from '@angular/common/http';
import { HttpRequest } from '@angular/common/http';
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
