import { ErrorHandler, Injectable, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AccountService } from './account.service';

@Injectable({
    providedIn: 'root'
})
export class HttpErrorHandler implements ErrorHandler {

    private url: string;

    constructor(
        private location: Location,
        private http: HttpClient,
        @Inject('apiRoot') apiRoot: string,
        private account: AccountService
    ) {
        this.url = `${apiRoot}/client-error`;
    }

    public handleError(error: any): void {
        const err: ErrorModel = {
            userName: this.account.info.getValue().userName,
            occuredAt: new Date(),
            userAgent: navigator.userAgent,
            path: this.location.path(),
            message: JSON.stringify(error)
        };
        this.http.post(this.url, err).toPromise().catch(ex => {
            console.error('Can not send error to server. ', err);
        });
    }

}

export interface ErrorModel {
    id?: string;
    userName?: string;
    occuredAt?: Date;
    userAgent?: string;
    path?: string;
    message?: string;
}
