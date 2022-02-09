import { Injectable, Inject, ErrorHandler } from '@angular/core';
import {
    HttpClient, HttpErrorResponse, HttpParams
} from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** 数据源服务 */
@Injectable({
    providedIn: 'root'
})
export class DataSourceService {

    public searchModel: DataSourceSearchModel = {
        keywords: '',
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<DataSourceModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl = `${this.apiRoot}/datasources`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) { }

    /** 搜索数据源 */
    public async search(): Promise<void> {
        let params = new HttpParams();
        for (const key in this.searchModel) {
            if (this.searchModel.hasOwnProperty(key)) {
                const val = this.searchModel[key] as string;
                params = params.set(key, val);
            }
        }
        this.loading = true;
        try {
            const result = await lastValueFrom(
                this.http.get<DataSourceResultModel>(this.baseUrl, { params })
            );
            const total = result.total ?? 0;
            const data = result.data ?? [];
            this.total.next(total);
            this.data.next(data);
            this.showPagination = total > data.length;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.total.next(0);
            this.data.next([]);
            this.ui.showAlert(
                { type: 'danger', message: '加载数据源数据出错!'}
            );
        }
        finally {
            this.loading = false;
        }
    }

    /** 加载全部数据源 */
    public async getAll(): Promise<DataSourceModel[]> {
        try {
            const result = await lastValueFrom(
                this.http.get<DataSourceModel[]>(`${this.apiRoot}/datasources-list`) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '加载全部数据源出错！' }
            );
            return [];
        }
    }

    /** 更改页码分页查询 */
    public async onPageChange(p: number): Promise<void> {
        this.searchModel.skip = (p - 1) * this.searchModel.take;
        await this.search();
    }

    /** 更改分页大小 */
    public async onPageSizeChange(): Promise<void> {
        this.searchModel.skip = 0;
        await this.search();
    }

    /** 创建数据源 */
    public async create(
        model: DataSourceModel
    ): Promise<DataSourceModel | undefined> {
        try {
            const result = lastValueFrom(
                await this.http.post<DataSourceModel>(this.baseUrl, model)
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建数据源出错！' }
            );
            return;
        }
    }

    /** 获取指定的数据源 */
    public async getById(id: string): Promise<DataSourceModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<DataSourceModel>(`${this.baseUrl}/${id}`)
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的数据源出错！' }
            );
            return;
        }
    }

    /** 删除数据源 */
    public async delete(id: string): Promise<boolean> {
        const confirm = await this.ui.showConfirm('确认删除么？');
        if (!confirm) {
            return false;
        }
        try {
            await lastValueFrom(
                this.http.delete(`${this.baseUrl}/${id}`)
            );
            return true;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除数据源出错！' }
            );
            return false;
        }
    }

    /** 更新数据源 */
    public async update(
        id: string,
        model: DataSourceModel
    ): Promise<DataSourceModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<DataSourceModel>(`${this.baseUrl}/${id}`, model)
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新数据源出错！' }
            );
            return;
        }
    }

    public checkStatus(model: DataSourceModel): Promise<StatusResult> {
        return new Promise<StatusResult>((resolve) => {
            const url = this.baseUrl + '-status';
            this.http.post<any>(url, model).subscribe({
                next: () => {
                    resolve({ status: 'success', message: '连接成功！' });
                },
                error: (res: HttpErrorResponse) => {
                    resolve({
                        status: res.status === 400 ? 'warning' : 'danger',
                        message: res.error as string
                    });
                }
            });
        });
    }

}

/** 数据源 */
export interface DataSourceModel {
    /** 数据源ID */
    id: string;
    /** 数据源名称 */
    name?: string;
    /** 数据库类型（postgres、mssql、mysql、oracle、sqlite等） */
    databaseType?: string;
    /** 服务器地址 */
    serverAddress?: string;
    /** 服务器端口 */
    serverPort?: number;
    /** 数据库名称 */
    databaseName?: string;
    /** 用户名 */
    username?: string;
    /** 密码 */
    password?: string;
    /** 超时时间（秒） */
    timeout?: number;
    /** 使用 ssl 安全连接 */
    useSsl?: boolean;
}

/** 数据源 搜索参数 */
export interface DataSourceSearchModel {
    [key: string]: undefined | number | string;
    keywords: string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
}

/** 数据源 搜索结果 */
export interface DataSourceResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: DataSourceModel[];
}

export interface StatusResult {
    status: 'info' | 'danger' | 'success' | 'warning';
    message: string;
}
