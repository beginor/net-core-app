import { ErrorHandler, Injectable, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';

import { AccountService } from './account.service';

@Injectable()
export class HttpErrorHandler implements ErrorHandler {

    private url: string;

    constructor(
        private location: Location,
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        @Inject('isProduction') private isProduction: boolean,
        private account: AccountService
    ) {
        this.url = `${apiRoot}/client-errors`;
    }

    public handleError(error: unknown): void {
        const err: ErrorModel = {
            userName: this.account.info.getValue().userName,
            occuredAt: new Date(),
            userAgent: navigator.userAgent,
            path: this.location.path(),
            message: JSON.stringify(error)
        };
        if (this.isProduction) {
            lastValueFrom(this.http.post(this.url, err)).catch(ex => {
                console.error('Can not send error to server. ', err);
            });
        }
        else {
            console.error(error);
        }
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
