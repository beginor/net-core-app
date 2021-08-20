import { ErrorHandler, Inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { lastValueFrom } from 'rxjs';

import { UiService } from './ui.service';

@Injectable({
    providedIn: 'root'
})
export class ServerFolderService {

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private errorHandler: ErrorHandler,
        private ui: UiService
    ) { }

    public async getFolderContent(
        params: ServerFolderContent
    ): Promise<ServerFolderContent> {
        try {
            const url = `${this.apiRoot}/server-folders/${params.alias}/browse`;
            const httpParams = new HttpParams()
                .set('path', params.path as string)
                .set('filter', params.filter as string);
            const result = await lastValueFrom(
                this.http.get<ServerFolderContent>(url, { params: httpParams })
            );
            return result;
        }
        catch (ex) {
            this.ui.showAlert(
                { type: 'warning', message: '无法获取服务器目录内容！' }
            );
            this.errorHandler.handleError(ex);
            return params;
        }
    }

}

export interface ServerFolderContent {
    alias: string;
    path: string;
    filter?: string;
    folders?: string[];
    files?: string[];
}
