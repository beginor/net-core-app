import { ErrorHandler, Inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from './ui.service';

/** 数据资源服务 */
@Injectable({ providedIn: 'root' })
export class ResourceService {

    private baseUrl: string;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) {
        this.baseUrl = `${apiRoot}/resources`;
    }

    public async getResourceCount(
        type?: string
    ): Promise<CategoryCountModel[]> {
        try {
            let params = new HttpParams()
            if (!!type) {
                params = params.set('type', type);
            }
            const result = await lastValueFrom(
                this.http.get<CategoryCountResultModel>(
                    `${this.baseUrl}/count/category`,
                    { params }
                )
            );
            return result.data;
        }
        catch (ex: unknown) {
            this.ui.showAlert(
                { type: 'warning', message: '无法获取类别统计数据！' }
            );
            this.errorHandler.handleError(ex);
            return [];
        }
    }

}

export interface CategoryCountModel {
    categoryId: string;
    categoryName: string;
    count: number;
}

export interface CategoryCountResultModel {
    data: CategoryCountModel[];
}
