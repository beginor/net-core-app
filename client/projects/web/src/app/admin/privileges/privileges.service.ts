import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

/** 系统权限 服务 */
@Injectable({
    providedIn: 'root'
})
export class AppPrivilegeService {

    public searchModel: AppPrivilegeSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<AppPrivilegeModel[]>([]);

    private baseUrl = `${this.apiRoot}/app-privileges`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string
    ) { }

    /** 搜索 系统权限 */
    public async search(): Promise<void> {
        let params = new HttpParams();
        for (const key in this.searchModel) {
            if (this.searchModel.hasOwnProperty(key)) {
                const val = this.searchModel[key];
                params = params.set(key, val);
            }
        }
        const result = await this.http.get<AppPrivilegeResultModel>(
            this.baseUrl,
            {
                params: params
            }
        ).toPromise();
        this.total.next(result.total);
        this.data.next(result.data);
    }

    /** 创建 系统权限 */
    public async create(model: AppPrivilegeModel): Promise<AppPrivilegeModel> {
        const result = await this.http.post<AppPrivilegeModel>(
            this.baseUrl,
            model
        ).toPromise();
        return result;
    }

    /** 获取指定的 系统权限 */
    public async getById(id: string): Promise<AppPrivilegeModel> {
        const result = await this.http.get<AppPrivilegeModel>(
            `${this.baseUrl}/${id}`
        ).toPromise();
        return result;
    }

    /** 删除 系统权限 */
    public async delete(id: string): Promise<void> {
        await this.http.delete(
            `${this.baseUrl}/${id}`
        ).toPromise();
    }

    /** 更新 系统权限 */
    public async update(
        id: string,
        model: AppPrivilegeModel
    ): Promise<AppPrivilegeModel> {
        const result = await this.http.put<AppPrivilegeModel>(
            `${this.baseUrl}/${id}`,
            model
        ).toPromise();
        return result;
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
    /** 跳过的记录数 */
    skip?: number;
    /** 取多少条记录 */
    take?: number;
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
