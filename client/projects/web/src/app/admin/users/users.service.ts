import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

@Injectable({
    providedIn: 'root'
})
export class UsersService {

    public searchModel: UserSearchModel = { skip: 0, take: 10 };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<UserModel[]>([]);
    public loading: boolean;

    private baseUrl = this.apiRoot + '/users';

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService
    ) {}

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
            const result = await this.http.get<UserSearchResult>(
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
            this.ui.showAlert({ type: 'danger', message: '加载用户数据出错!'});
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

    /** 创建用户 */
    public async create(model: UserModel): Promise<UserModel> {
        try {
            const result = await this.http.post<UserModel>(
                this.baseUrl,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert({ type: 'danger', message: '创建用户出错！' });
            return null;
        }
    }

    /** 获取指定的用户 */
    public async getById(id: string): Promise<UserModel> {
        try {
            const result = await this.http.get<UserModel>(
                `${this.baseUrl}/${id}`
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert({ type: 'danger', message: '获取指定的用户出错！' });
            return null;
        }
    }

    /** 删除用户 */
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
            this.ui.showAlert({ type: 'danger', message: '删除用户出错！' });
            return false;
        }
    }

    /** 更新用户 */
    public async update(
        id: string,
        model: UserModel
    ): Promise<UserModel> {
        try {
            const result = await this.http.put<UserModel>(
                `${this.baseUrl}/${id}`,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert({ type: 'danger', message: '更新用户出错！' });
            return null;
        }
    }

}

/**
 * 应用程序用户模型
 */
export interface UserModel {
    /** 用户名 */
    userName?: string;
    /** 电子邮箱地址 */
    email?: string;
    /** 电子邮箱地址是否已确认 */
    emailConfirmed?: boolean;
    /** 电话号码 */
    phoneNumber?: string;
    /** 电话号码是否已经确认 */
    phoneNumberConfirmed?: boolean;
    /** 是否允许（自动）锁定 */
    lockoutEnabled?: boolean;
    /** 锁定结束时间 */
    lockoutEnd?: string;
    /** 登录失败次数 */
    accessFailedCount?: number;
    /** 是否启用两部认证 */
    twoFactorEnabled?: boolean;
    /** 创建时间 */
    createTime?: string;
    /** 最近登录时间 */
    lastLogin?: string;
    /** 登录次数 */
    loginCount?: number;
}

/** 用户搜索参数 */
export interface UserSearchModel {
    /** 跳过的记录数 */
    skip?: number;
    /** 取多少条记录 */
    take?: number;
    /** 用户名 */
    userName?: string;
}

/** 用户搜索结果 */
export interface UserSearchResult {
    /** 用户列表 */
    data?: UserModel[];
    /** 总记录数 */
    total?: number;
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求取多少条记录 */
    take?: number;
}
