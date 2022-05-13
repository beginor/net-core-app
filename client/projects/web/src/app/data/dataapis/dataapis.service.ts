import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { AccountService, makeAbsoluteUrl, UserTokenModel } from 'app-shared';
import { UiService } from 'projects/web/src/app/common';
import { RolesService, AppRoleModel } from '../../admin/roles/roles.service';

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
    public roles: AppRoleModel[] = [];
    public tokens = new BehaviorSubject<UserTokenModel[]>([]);

    private baseUrl = `${this.apiRoot}/dataapis`;
    private rolesSvc: RolesService;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private account: AccountService,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) {
        this.rolesSvc = new RolesService(http, apiRoot, ui, errorHandler);
        this.rolesSvc.data.subscribe(data => this.roles = data);
    }

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

    public async getAllRoles(): Promise<void> {
        try {
            this.rolesSvc.searchModel.skip = 0;
            this.rolesSvc.searchModel.take = 999;
            await this.rolesSvc.search();
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取角色列表出错！' }
            );
        }
    }

    public async getColumns(
        id: string
    ): Promise<DataApiColumnModel[] | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<DataApiColumnModel[]>(`${this.baseUrl}/${id}/columns`) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            let message = '获取接口输出字段出错！';
            const { error } = ex;
            if (!!error) {
                message += `错误信息为： ${error}`;
            }
            this.ui.showAlert(
                { type: 'danger', message }
            );
            return;
        }
    }

    public getApiUrl(id: string, type: ResultType): string {
        const url = `${this.baseUrl}/${id}/${type}`;
        return makeAbsoluteUrl(url);
    }

    public async loadTokens(): Promise<void> {
        try {
            const result = await this.account.searchUserTokens(
                { skip: 0, take: 999 }
            );
            this.tokens.next(result.data ?? []);
        }
        catch (ex: any) {
            this.tokens.next([]);
            this.ui.showAlert(
                { type: 'danger', message: '获取用户凭证出错！' }
            );
            this.errorHandler.handleError(ex);
            console.error(ex, '获取用户凭证出错！');
        }
    }

    public async exportApiDoc(model: DataApiDocModel): Promise<string> {
        let httpParams = new HttpParams()
            .set('title', model.title)
            .set('format', model.format)
            .set('token', model.token);
        for (const id of model.apis) {
            httpParams = httpParams.append('id', id);
        }
        if (!!model.description) {
            httpParams = httpParams.set('description', model.description);
        }
        if (!!model.referer) {
            httpParams = httpParams.set('referer', model.referer)
        }
        try {
            const url = `${this.baseUrl}-doc?${httpParams.toString()}`;
            const result = await lastValueFrom(
                this.http.get(url, { responseType: 'text'})
            )
            return result;
        }
        catch (ex: any) {
            this.ui.showAlert(
                { type: 'danger', message: '导出接口文档出错！' }
            );
            this.errorHandler.handleError(ex);
            console.error(ex);
            return '';
        }
    }

}

export interface DataApiDocModel {
    title: string,
    description?: string;
    apis: string[],
    format: string;
    token: string;
    referer?: string;
}

export type ResultType = 'data' | 'columns' | 'sql' | 'geojson';

/** 数据API */
export interface DataApiModel {
    /** 数据API ID */
    id: string;
    /** 数据API名称 */
    name?: string;
    /** 数据API描述 */
    description?: string;
    category?: { id?: string; name?: string; };
    /** 允许访问的角色 */
    roles?: string[];
    tags?: string[];
    /** 创建者 */
    creator?: { id?: string; name?: string; };
    /** 创建时间 */
    createdAt?: string;
    /** 更新者 */
    updater?: { id?: string; name?: string; };
    /** 更新时间 */
    updatedAt?: string;
    /** 数据源 */
    dataSource?: { id?: string; name?: string; };
    /** 是否向数据源写入数据 */
    writeData?: boolean;
    /** 数据API调用的 XML + SQL 命令 */
    statement?: string;
    /** 参数定义 */
    parameters?: DataApiParameterModel[];
    /** API 输出列的元数据 */
    columns?: DataApiColumnModel[];
    /** 输出列中的标识列 */
    idColumn?: string;
    /** 输出列中的空间列 */
    geometryColumn?: string;
}

/** 数据API 搜索参数 */
export interface DataApiSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
    /** 搜索关键字 */
    keywords?: string;
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
