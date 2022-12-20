import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** slpk 航拍模型服务 */
@Injectable({
    providedIn: 'root'
})
export class SlpkService {

    public searchModel: SlpkSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<SlpkModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl: string;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) {
        this.baseUrl = `${apiRoot}/slpks`;
    }

    /** 搜索slpk 航拍模型 */
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
                this.http.get<SlpkResultModel>(this.baseUrl, { params })
            );
            const total = result.total ?? 0;
            const data = result.data ?? [];
            this.total.next(total);
            this.data.next(data);
            this.showPagination = total > data.length;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.total.next(0);
            this.data.next([]);
            this.ui.showAlert(
                { type: 'danger', message: '加载slpk 航拍模型数据出错!'}
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

    /** 创建slpk 航拍模型 */
    public async create(
        model: SlpkModel
    ): Promise<SlpkModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.post<SlpkModel>(this.baseUrl, model)
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建slpk 航拍模型出错！' }
            );
            return;
        }
    }

    /** 获取指定的slpk 航拍模型 */
    public async getById(id: string): Promise<SlpkModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<SlpkModel>(`${this.baseUrl}/${id}`)
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的slpk 航拍模型出错！' }
            );
            return;
        }
    }

    /** 删除slpk 航拍模型 */
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
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除slpk 航拍模型出错！' }
            );
            return false;
        }
    }

    /** 更新slpk 航拍模型 */
    public async update(
        id: string,
        model: SlpkModel
    ): Promise<SlpkModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<SlpkModel>(`${this.baseUrl}/${id}`, model)
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新slpk 航拍模型出错！' }
            );
            return;
        }
    }

    public getSlpkLayerUrl(id: string): string {
        if (!id) {
            return '';
        }
        let url = this.apiRoot.substring(0, this.apiRoot.length - 3);
        if (url.startsWith('/')) {
            url = `${location.protocol}//${location.host}${url}`;
        }
        url = `${url}rest/services/slpks/${id}/SceneServer`;
        return url;
    }

}

/** slpk 航拍模型 */
export interface SlpkModel {
    /** 航拍模型id */
    id: string;
    /** 模型名称 */
    name?: string;
    /** 模型描述 */
    description?: string;
    /** 类别 */
    category?: { id?: string; name?: string; };
    /** 角色 */
    roles?: string[];
    /** 标签 */
    tags?: string[];
    /** 航拍模型目录 */
    directory?: string;
    /** 模型经度 */
    longitude?: number;
    /** 模型纬度 */
    latitude?: number;
    /** 模型海拔高度 */
    elevation?: number;
}

/** slpk 航拍模型 搜索参数 */
export interface SlpkSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
    keywords?: string;
    category?: string;
}

/** slpk 航拍模型 搜索结果 */
export interface SlpkResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: SlpkModel[];
}
