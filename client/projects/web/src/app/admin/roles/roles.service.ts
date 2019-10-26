import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

import { UiService} from 'projects/web/src/app/common';

/** 角色服务 */
@Injectable({
    providedIn: 'root'
})
export class RoleService {

    public searchModel: AppRoleSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<AppRoleModel[]>([]);
    public loading: boolean;

    private baseUrl = `${this.apiRoot}/roles`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService
    ) { }

    /** 搜索角色 */
    public async search(): Promise<void> {
        let params = new HttpParams();
        for (const key in this.searchModel) {
            if (this.searchModel.hasOwnProperty(key)) {
                const val = this.searchModel[key];
                params = params.set(key, val);
            }
        }
        this.loading = true;
        try {
            const result = await this.http.get<AppRoleResultModel>(
                this.baseUrl,
                {
                    params: params
                }
            ).toPromise();
            this.total.next(result.total);
            this.data.next(result.data);
        }
        catch (ex) {
            console.error(ex.toString());
            this.total.next(0);
            this.data.next([]);
            this.ui.showAlert({ type: 'danger', message: '加载角色数据出错!'});
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

    /** 创建角色 */
    public async create(model: AppRoleModel): Promise<AppRoleModel> {
        try {
            const result = await this.http.post<AppRoleModel>(
                this.baseUrl,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert({ type: 'danger', message: '创建角色出错！' });
            return null;
        }
    }

    /** 获取指定的角色 */
    public async getById(id: string): Promise<AppRoleModel> {
        try {
            const result = await this.http.get<AppRoleModel>(
                `${this.baseUrl}/${id}`
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert({ type: 'danger', message: '获取指定的角色出错！' });
            return null;
        }
    }

    /** 删除角色 */
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
            this.ui.showAlert({ type: 'danger', message: '删除角色出错！' });
            return false;
        }
    }

    /** 更新角色 */
    public async update(
        id: string,
        model: AppRoleModel
    ): Promise<AppRoleModel> {
        try {
            const result = await this.http.put<AppRoleModel>(
                `${this.baseUrl}/${id}`,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert({ type: 'danger', message: '更新角色出错！' });
            return null;
        }
    }

}

/** 角色 */
export interface AppRoleModel {
    /** 角色标识 */
    id?: string;
    /** 角色名称 */
    name?: string;
    /** 角色描述 */
    description?: string;
    /** 是否默认 */
    isDefault?: boolean;
    userCount?: number;
}

/** 角色 搜索参数 */
export interface AppRoleSearchModel {
    /** 跳过的记录数 */
    skip?: number;
    /** 取多少条记录 */
    take?: number;
}

/** 角色 搜索结果 */
export interface AppRoleResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: AppRoleModel[];
}
