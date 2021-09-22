import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** 用户凭证服务 */
@Injectable({
    providedIn: 'root'
})
export class AppUserTokenService {

    public searchModel: AppUserTokenSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<AppUserTokenModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl = `${this.apiRoot}/app-user-tokens`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) { }

    /** 搜索用户凭证 */
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
                this.http.get<AppUserTokenResultModel>(this.baseUrl, { params }) // eslint-disable-line max-len
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
                { type: 'danger', message: '加载用户凭证数据出错!'}
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

    /** 创建用户凭证 */
    public async create(
        model: AppUserTokenModel
    ): Promise<AppUserTokenModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.post<AppUserTokenModel>(this.baseUrl, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建用户凭证出错！' }
            );
            return;
        }
    }

    /** 获取指定的用户凭证 */
    public async getById(id: string): Promise<AppUserTokenModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<AppUserTokenModel>(`${this.baseUrl}/${id}`) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的用户凭证出错！' }
            );
            return;
        }
    }

    /** 删除用户凭证 */
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
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除用户凭证出错！' }
            );
            return false;
        }
    }

    /** 更新用户凭证 */
    public async update(
        id: string,
        model: AppUserTokenModel
    ): Promise<AppUserTokenModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<AppUserTokenModel>(`${this.baseUrl}/${id}`, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新用户凭证出错！' }
            );
            return;
        }
    }

}

/** 用户凭证 */
export interface AppUserTokenModel {
    /** 凭证id */
    id: string;
    /** 用户id */
    userId?: string;
    /** 凭证名称 */
    name?: string;
    /** 凭证值 */
    value?: string;
    /** 凭证权限 */
    privileges?: string[];
    /** 允许的 url 地址 */
    urls?: string[];
    /** 更新时间 */
    updateTime?: string;
    /** 过期时间 */
    expiresAt?: string;
    /** 凭证代表的角色 */
    roles?: string[];
}

/** 用户凭证 搜索参数 */
export interface AppUserTokenSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
}

/** 用户凭证 搜索结果 */
export interface AppUserTokenResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: AppUserTokenModel[];
}
