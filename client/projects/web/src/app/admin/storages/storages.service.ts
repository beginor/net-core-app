import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';
import { RolesService, AppRoleModel } from '../roles/roles.service';

/** 应用存储服务 */
@Injectable({
    providedIn: 'root'
})
export class AppStorageService {

    public searchModel: AppStorageSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<AppStorageModel[]>([]);
    public loading = false;
    public showPagination = false;
    public roles: AppRoleModel[] = [];

    private baseUrl = `${this.apiRoot}/storages`;
    private rolesSvc: RolesService;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) {
        this.rolesSvc = new RolesService(
            http, apiRoot, ui, errorHandler
        );
        this.rolesSvc.data.subscribe(data => {
            this.roles = data;
        });
    }

    /** 搜索应用存储 */
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
                this.http.get<AppStorageResultModel>(this.baseUrl, { params }) // eslint-disable-line max-len
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
                { type: 'danger', message: '加载应用存储数据出错!'}
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

    /** 创建应用存储 */
    public async create(
        model: AppStorageModel
    ): Promise<AppStorageModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.post<AppStorageModel>(this.baseUrl, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建应用存储出错！' }
            );
            return;
        }
    }

    /** 获取指定的应用存储 */
    public async getById(id: string): Promise<AppStorageModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<AppStorageModel>(`${this.baseUrl}/${id}`) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的应用存储出错！' }
            );
            return;
        }
    }

    /** 删除应用存储 */
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
                { type: 'danger', message: '删除应用存储出错！' }
            );
            return false;
        }
    }

    /** 更新应用存储 */
    public async update(
        id: string,
        model: AppStorageModel
    ): Promise<AppStorageModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<AppStorageModel>(`${this.baseUrl}/${id}`, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新应用存储出错！' }
            );
            return;
        }
    }

    public async getAllRoles(): Promise<void> {
        try {
            this.rolesSvc.searchModel.skip = 0;
            this.rolesSvc.searchModel.take = 999;
            await this.rolesSvc.search();
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert({ type: 'danger', message: '获取全部角色出错！' });
        }
    }

}

/** 应用存储 */
export interface AppStorageModel {
    /** 应用存储id */
    id: string;
    /** 存储别名 */
    aliasName: string;
    /** 存储根路径 */
    rootFolder: string;
    /** 是否只读 */
    readonly: boolean;
    /** 可访问此存储的角色 */
    roles?: string[];
}

/** 应用存储 搜索参数 */
export interface AppStorageSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
    keywords?: string;
}

/** 应用存储 搜索结果 */
export interface AppStorageResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: AppStorageModel[];
}
