import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap';
import { BehaviorSubject } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

@Injectable({
    providedIn: 'root'
})
export class AuditLogsService {

    public searchModel: AuditLogSearchModel = {
        skip: 0,
        take: 10
    };
    public searchDate: NgbDate;
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<AuditLogModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl = `${this.apiRoot}/audit-logs`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) {
        const today = new Date();
        this.searchDate = new NgbDate(
            today.getFullYear(),
            today.getMonth() + 1,
            today.getDate()
        );
    }

    public async search(): Promise<void> {
        const d = this.searchDate;
        this.searchModel.requestDate = `${d.year}-${d.month}-${d.day}`;
        let params = new HttpParams();
        for (const key in this.searchModel) {
            if (this.searchModel.hasOwnProperty(key)) {
                const val = this.searchModel[key];
                params = params.set(key, val as string);
            }
        }
        this.loading = true;
        try {
            const result = await this.http.get<AuditLogResultModel>(
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
            this.loading = false;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.total.next(0);
            this.data.next([]);
            this.ui.showAlert({ type: 'danger', message: '加载审计日志数据出错!'});
        }
        finally {
            this.loading = false;
        }
    }

    public async create(model: AuditLogModel): Promise<AuditLogModel> {
        const result = await this.http.post<AuditLogModel>(
            this.baseUrl,
            model
        ).toPromise();
        return result;
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
    /** 客户端 IP 地址 */
    ip?: string;
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
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
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
