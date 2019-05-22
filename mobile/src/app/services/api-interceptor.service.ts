import { Injectable } from '@angular/core';
import {
    HttpInterceptor, HttpEvent, HttpRequest, HttpHandler
} from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

@Injectable()
export class ApiInterceptorService implements HttpInterceptor {

    public intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        if (req.url.startsWith(environment.apiRoot)) {
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
