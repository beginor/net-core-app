import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** 数据类别服务 */
@Injectable({
    providedIn: 'root'
})
export class CategoryService {

    public searchModel: CategorySearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<CategoryModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl = `${this.apiRoot}/categories`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) { }

    /** 搜索数据类别 */
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
                this.http.get<CategoryResultModel>(this.baseUrl, { params }) // eslint-disable-line max-len
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
                { type: 'danger', message: '加载数据类别数据出错!'}
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

    /** 创建数据类别 */
    public async create(
        model: CategoryModel
    ): Promise<CategoryModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.post<CategoryModel>(this.baseUrl, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建数据类别出错！' }
            );
            return;
        }
    }

    /** 获取指定的数据类别 */
    public async getById(id: string): Promise<CategoryModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<CategoryModel>(`${this.baseUrl}/${id}`) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的数据类别出错！' }
            );
            return;
        }
    }

    /** 删除数据类别 */
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
                { type: 'danger', message: '删除数据类别出错！' }
            );
            return false;
        }
    }

    /** 更新数据类别 */
    public async update(
        id: string,
        model: CategoryModel
    ): Promise<CategoryModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<CategoryModel>(`${this.baseUrl}/${id}`, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新数据类别出错！' }
            );
            return;
        }
    }

}

/** 数据类别 */
export interface CategoryModel {
    /** 类别ID */
    id: string;
    /** 类别名称 */
    name: string;
    /** 父类ID */
    parentId?: string;
    /** 顺序号 */
    sequence: number;
}

/** 数据类别 搜索参数 */
export interface CategorySearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
}

/** 数据类别 搜索结果 */
export interface CategoryResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: CategoryModel[];
}