import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** 数据API服务 */
@Injectable({
    providedIn: 'root'
})
export class DataApiService {

    public searchModel: DataApiSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<DataApiModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl = `${this.apiRoot}/dataapis`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) { }

    /** 搜索数据API */
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
                this.http.get<DataApiResultModel>(this.baseUrl, { params }) // eslint-disable-line max-len
            );
            const total = result.total ?? 0;
            const data = result.data ?? [];
            this.total.next(total);
            this.data.next(data);
            this.showPagination = total > data.length;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.total.next(0);
            this.data.next([]);
            this.ui.showAlert(
                { type: 'danger', message: '加载数据API数据出错!'}
            );
        }
        finally {
            this.loading = false;
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

    /** 创建数据API */
    public async create(
        model: DataApiModel
    ): Promise<DataApiModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.post<DataApiModel>(this.baseUrl, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建数据API出错！' }
            );
            return;
        }
    }

    /** 获取指定的数据API */
    public async getById(id: string): Promise<DataApiModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<DataApiModel>(`${this.baseUrl}/${id}`) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的数据API出错！' }
            );
            return;
        }
    }

    /** 删除数据API */
    public async delete(id: string): Promise<boolean> {
        const confirm = await this.ui.showConfirm('确认删除么？');
        if (!confirm) {
            return false;
        }
        try {
            await lastValueFrom(
                this.http.delete(`${this.baseUrl}/${id}`) // eslint-disable-line max-len
            );
            return true;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除数据API出错！' }
            );
            return false;
        }
    }

    /** 更新数据API */
    public async update(
        id: string,
        model: DataApiModel
    ): Promise<DataApiModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<DataApiModel>(`${this.baseUrl}/${id}`, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新数据API出错！' }
            );
            return;
        }
    }

}

/** 数据API */
export interface DataApiModel {
    /** 数据API ID */
    id: string;
    /** 数据API名称 */
    name?: string;
    /** 数据API描述 */
    description?: string;
    /** 数据源 */
    dataSource?: { id: string; name: string; };
    /** 是否向数据源写入数据 */
    writeData?: boolean;
    /** 数据API调用的 XML + SQL 命令 */
    statement?: string;
    /** 参数定义 */
    parameters?: DataApiParameterModel[];
    /** API 输出列的源数据 */
    columns?: DataApiColumnModel[];
    /** 允许访问的角色 */
    roles?: string[];
    /** 创建者 */
    creator?: { id: string; name: string; };
    /** 创建时间 */
    createdAt?: string;
    /** 更新者 */
    updater?: { id: string; name: string; };
    /** 更新时间 */
    updatedAt?: string;
}

/** 数据API 搜索参数 */
export interface DataApiSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
}

/** 数据API 搜索结果 */
export interface DataApiResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: DataApiModel[];
}

export interface DataApiParameterModel {
    name?: string;
    type?: string;
    description?: string;
    source?: string;
    required?: boolean;
}

export interface DataApiColumnModel {
    name?: string;
    description?: string;
    type?: string;
}
