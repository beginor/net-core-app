import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

import { UiService} from 'projects/web/src/app/common';

/** 系统权限 服务 */
@Injectable({
    providedIn: 'root'
})
export class AppPrivilegeService {

    public searchModel: AppPrivilegeSearchModel = {
        skip: 0,
        take: 10,
        module: ''
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<AppPrivilegeModel[]>([]);
    public modules = new BehaviorSubject<string[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl = `${this.apiRoot}/privileges`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) { }

    /** 搜索系统权限 */
    public async search(): Promise<void> {
        let params = new HttpParams();
        for (const key in this.searchModel) {
            if (this.searchModel.hasOwnProperty(key)) {
                const val = this.searchModel[key];
                if (!!val) {
                    params = params.set(key, val as string);
                }
            }
        }
        this.loading = true;
        try {
            const result = await this.http.get<AppPrivilegeResultModel>(
                this.baseUrl,
                { params: params }
            ).toPromise();
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
            this.ui.showAlert({ type: 'danger', message: '加载系统权限数据出错！' });
        }
        finally {
            this.loading = false;
        }
    }

    public async onPageChange(p: number): Promise<void> {
        this.searchModel.skip = (p - 1) * this.searchModel.take;
        await this.search();
    }

    public async onPageSizeChange(): Promise<void> {
        this.searchModel.skip = 0;
        await this.search();
    }

    /** 创建 系统权限 */
    public async create(
        model: AppPrivilegeModel
    ): Promise<AppPrivilegeModel | undefined> {
        try {
            const result = await this.http.post<AppPrivilegeModel>(
                this.baseUrl,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert({ type: 'danger', message: '创建系统权限出错！' });
            return;
        }
    }

    /** 获取指定的 系统权限 */
    public async getById(id: string): Promise<AppPrivilegeModel | undefined> {
        try {
            const result = await this.http.get<AppPrivilegeModel>(
                `${this.baseUrl}/${id}`
            ).toPromise();
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert({ type: 'danger', message: '加载系统权限数据出错！' });
            return;
        }
    }

    /** 删除 系统权限 */
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
            this.errorHandler.handleError(ex);
            this.ui.showAlert({ type: 'danger', message: '删除系统权限出错！' });
            return false;
        }
    }

    /** 更新 系统权限 */
    public async update(
        id: string,
        model: AppPrivilegeModel
    ): Promise<AppPrivilegeModel | undefined> {
        try {
            const result = await this.http.put<AppPrivilegeModel>(
                `${this.baseUrl}/${id}`,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert({ type: 'danger', message: '更新系统权限出错！' });
            return;
        }
    }

    /** 获取模块名称 */
    public async getModules(): Promise<void> {
        try {
            const modules = await this.http.get<string[]>(
                `${this.apiRoot}/modules`
            ).toPromise();
            this.modules.next(modules);
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert({ type: 'danger', message: '加载模块列表出错!'});
            this.modules.next([]);
        }
    }

}

/** 系统权限 */
export interface AppPrivilegeModel {
    /** 权限ID */
    id?: string;
    /** 权限模块 */
    module?: string;
    /** 权限名称( Identity 的策略名称) */
    name?: string;
    /** 权限描述 */
    description?: string;
    /** 是否必须。 与代码中的 Authorize 标记对应的权限为必须的权限， 否则为可选的。 */
    isRequired?: boolean;
}

/** 系统权限 搜索参数 */
export interface AppPrivilegeSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
    /** 模块 */
    module?: string;
}

/** 系统权限 搜索结果 */
export interface AppPrivilegeResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: AppPrivilegeModel[];
}
