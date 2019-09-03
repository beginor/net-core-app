import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AuditLogsService {
    public searchModel: AuditLogSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<AuditLogModel[]>([]);

    private baseUrl = `${this.apiRoot}/app-audit-logs`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string
    ) { }

    public async search(): Promise<void> {
        let params = new HttpParams();
        for (const key in this.searchModel) {
            if (this.searchModel.hasOwnProperty(key)) {
                const val = this.searchModel[key];
                params = params.set(key, val);
            }
        }
        const result = await this.http.get<AuditLogResultModel>(
            this.baseUrl,
            {
                params: params
            }
        ).toPromise();
        this.total.next(result.total);
        this.data.next(result.data);
    }

    public async create(model: AuditLogModel): Promise<AuditLogModel> {
        const result = await this.http.post<AuditLogModel>(
            this.baseUrl,
            model
        ).toPromise();
        return result;
    }

    public async getById(id: string): Promise<AuditLogModel> {
        const result = await this.http.get<AuditLogModel>(
            `${this.baseUrl}/${id}`
        ).toPromise();
        return result;
    }

    public async delete(id: string): Promise<void> {
        await this.http.delete(
            `${this.baseUrl}/${id}`
        ).toPromise();
    }

    public async update(
        id: string,
        model: AuditLogModel
    ): Promise<AuditLogModel> {
        const result = await this.http.put<AuditLogModel>(
            `${this.baseUrl}/${id}`,
            model
        ).toPromise();
        return result;
    }
}

/** 审计日志模型 */
export interface AuditLogModel {
    /** 审计日志 ID */
    id?: string;
    /** 请求路径 */
    requestPath?: string;
    /** 请求方法 */
    requestMethod?: string;
    /** 用户名 */
    userName?: string;
    /** 开始时间 */
    startAt?: string;
    /** 耗时(毫秒) */
    duration?: number;
    /** 响应状态码 */
    responseCode?: number;
    /** 控制器名称 */
    controllerName?: string;
    /** 动作名称 */
    actionName?: string;
    /** 描述 */
    description?: string;
}

/** 审计日志搜索参数 */
export interface AuditLogSearchModel {
    /** 跳过的记录数 */
    skip?: number;
    /** 取多少条记录 */
    take?: number;
    /** 用户名 */
    userName?: string;
    /** 请求日期，精确到日 */
    requestDate?: string;
}

/** 审计日志搜索结果 */
export interface AuditLogResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 日志列表 */
    data?: AuditLogModel[];
}
