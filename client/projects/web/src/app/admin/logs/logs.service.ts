import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';
import {
    NgbDate, NgbCalendar, NgbDateParserFormatter
} from '@ng-bootstrap/ng-bootstrap';

import { UiService } from 'projects/web/src/app/common';

/** 应用程序日志服务 */
@Injectable({
    providedIn: 'root'
})
export class AppLogService {

    public searchModel: AppLogSearchModel = {
        skip: 0,
        take: 10,
        level: ''
    };
    public startDate: NgbDate;
    public endDate: NgbDate;
    public maxDate:NgbDate;
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<AppLogModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl: string;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler,
        private formatter: NgbDateParserFormatter,
        calendar: NgbCalendar
    ) {
        this.baseUrl = `${this.apiRoot}/logs`;
        const today = calendar.getToday();
        this.endDate = today;
        this.maxDate = today;
        this.startDate = calendar.getPrev(today, 'd', 3);
    }

    /** 搜索应用程序日志 */
    public async search(): Promise<void> {
        this.searchModel.startDate = this.formatter.format(this.startDate);
        this.searchModel.endDate = this.formatter.format(this.endDate);
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
                this.http.get<AppLogResultModel>(this.baseUrl, { params }) // eslint-disable-line max-len
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
                { type: 'danger', message: '加载运行日志数据出错!'}
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

    /** 获取指定的应用程序日志 */
    public async getById(id: string): Promise<AppLogModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<AppLogModel>(`${this.baseUrl}/${id}`) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的应用程序日志出错！' }
            );
            return;
        }
    }

    public getTextClass(level?: string): string {
        // if (level === 'DEBUG') {
        //     return 'text-muted';
        // }
        if (level === 'INFO') {
            return 'text-info';
        }
        if (level === 'WARN') {
            return 'text-warning';
        }
        if (level === 'ERROR') {
            return 'text-danger';
        }
        if (level === 'FATAL') {
            return 'text-danger';
        }
        return 'text-muted';
    }

}
/** 应用程序日志 */
export interface AppLogModel {
    /** 日志ID */
    id: string;
    /** 创建时间 */
    createdAt?: string;
    /** 线程ID */
    thread?: string;
    /** 日志级别 */
    level?: string;
    /** 记录者 */
    logger?: string;
    /** 日志消息 */
    message?: string;
    /** 异常信息 */
    exception?: string;
}

/** 应用程序日志 搜索参数 */
export interface AppLogSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
    /** 开始日期，精确到日 */
    startDate?: string;
    /** 结束日期， 精确到日 */
    endDate?: string;
    level?: string;
}

/** 应用程序日志 搜索结果 */
export interface AppLogResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: AppLogModel[];
}
