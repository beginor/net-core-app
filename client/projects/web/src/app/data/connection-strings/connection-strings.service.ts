import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** 数据库连接串服务 */
@Injectable({
    providedIn: 'root'
})
export class ConnectionStringService {

    public searchModel: ConnectionStringSearchModel = {
        keywords: '',
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<ConnectionStringModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl = `${this.apiRoot}/connection-strings`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService
    ) { }

    /** 搜索数据库连接串 */
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
            const result = await this.http.get<ConnectionStringResultModel>(
                this.baseUrl,
                {
                    params: params
                }
            ).toPromise();
            const total = result.total ?? 0;
            const data = result.data ?? [];
            this.total.next(total);
            this.data.next(data);
            this.showPagination = total > data.length;
        }
        catch (ex) {
            console.error(ex.toString());
            this.total.next(0);
            this.data.next([]);
            this.ui.showAlert(
                { type: 'danger', message: '加载数据库连接串数据出错!'}
            );
        }
        finally {
            this.loading = false;
        }
    }

    /** 加载全部数据库连接串 */
    public async getAll(): Promise<ConnectionStringModel[]> {
        try {
            const result = await this.http.get<ConnectionStringModel[]>(
                `${this.apiRoot}/connection-strings-list`
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex.toString());
            this.ui.showAlert(
                { type: 'danger', message: '加载全部数据库连接串出错！' }
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

    /** 创建数据库连接串 */
    public async create(
        model: ConnectionStringModel
    ): Promise<ConnectionStringModel | undefined> {
        try {
            const result = await this.http.post<ConnectionStringModel>(
                this.baseUrl,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建数据库连接串出错！' }
            );
            return;
        }
    }

    /** 获取指定的数据库连接串 */
    public async getById(id: string): Promise<ConnectionStringModel | undefined> {
        try {
            const result = await this.http.get<ConnectionStringModel>(
                `${this.baseUrl}/${id}`
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的数据库连接串出错！' }
            );
            return;
        }
    }

    /** 删除数据库连接串 */
    public async delete(id: string): Promise<boolean> {
        const confirm = await this.ui.showConfirm('确认删除么？');
        if (!confirm) {
            return false;
        }
        try {
            await this.http.delete(
                `${this.baseUrl}/${id}`
            ).toPromise();
            return true;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除数据库连接串出错！' }
            );
            return false;
        }
    }

    /** 更新数据库连接串 */
    public async update(
        id: string,
        model: ConnectionStringModel
    ): Promise<ConnectionStringModel | undefined> {
        try {
            const result = await this.http.put<ConnectionStringModel>(
                `${this.baseUrl}/${id}`,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新数据库连接串出错！' }
            );
            return;
        }
    }

}

/** 数据库连接串 */
export interface ConnectionStringModel {
    /** 连接串ID */
    id?: string;
    /** 连接串名称 */
    name?: string;
    /** 连接串值 */
    value?: string;
    /** 数据库类型（postgres、mssql、mysql、oracle、sqlite等） */
    databaseType?: string;
}

/** 数据库连接串 搜索参数 */
export interface ConnectionStringSearchModel {
    [key: string]: undefined | number | string;
    keywords: string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
}

/** 数据库连接串 搜索结果 */
export interface ConnectionStringResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: ConnectionStringModel[];
}
