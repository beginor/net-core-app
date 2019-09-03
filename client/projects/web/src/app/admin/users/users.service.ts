import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UsersService {

    public searchModel: UserSearchModel = { skip: 0, take: 10 };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<UserModel[]>([]);

    private baseUrl = this.apiRoot + '/users';

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string
    ) {}

    public async search(): Promise<void> {
        const result = await this.http.get<UserSearchResult>(
            this.baseUrl
        ).toPromise();
        this.total.next(result.total);
        this.data.next(result.data);
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
