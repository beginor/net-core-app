import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor } from '@angular/common/http';
import { HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';

export class ApiInterceptor implements HttpInterceptor {

    public intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        if (req.url.startsWith(environment.apiUrl)) {
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
