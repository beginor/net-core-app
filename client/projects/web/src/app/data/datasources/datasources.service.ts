import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';
import { ColumnModel } from './metadata.service';

/** 数据源（数据表或视图）服务 */
@Injectable({
    providedIn: 'root'
})
export class DataSourceService {

    public searchModel: DataSourceSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<DataSourceModel[]>([]);
    public loading = false;
    public showPagination = false;
    private baseUrl = `${this.apiRoot}/datasources`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService
    ) { }

    /** 搜索数据源（数据表或视图） */
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
            const result = await this.http.get<DataSourceResultModel>(
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
                { type: 'danger', message: '加载数据源（数据表或视图）数据出错!'}
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

    /** 创建数据源（数据表或视图） */
    public async create(
        model: DataSourceModel
    ): Promise<DataSourceModel | undefined> {
        try {
            const result = await this.http.post<DataSourceModel>(
                this.baseUrl,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建数据源出错！' }
            );
            return;
        }
    }

    /** 获取指定的数据源（数据表或视图） */
    public async getById(id: string): Promise<DataSourceModel | undefined> {
        try {
            const result = await this.http.get<DataSourceModel>(
                `${this.baseUrl}/${id}`
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的数据源（数据表或视图）出错！' }
            );
            return;
        }
    }

    /** 删除数据源（数据表或视图） */
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
                { type: 'danger', message: '删除数据源（数据表或视图）出错！' }
            );
            return false;
        }
    }

    /** 更新数据源（数据表或视图） */
    public async update(
        id: string,
        model: DataSourceModel
    ): Promise<DataSourceModel | undefined> {
        try {
            const result = await this.http.put<DataSourceModel>(
                `${this.baseUrl}/${id}`,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新数据源（数据表或视图）出错！' }
            );
            return;
        }
    }

    public async getColumns(id: string): Promise<ColumnModel[]> {
        try {
            const result = await this.http.get<ColumnModel[]>(
                `${this.baseUrl}/${id}/columns`
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取数据源的列数据出错！' }
            );
            return [];
        }
    }

    public async getData(
        id: string,
        params: ReadDataParam
    ): Promise<PaginatedResult> {
        try {
            let httpParams = new HttpParams();
            for (const key in params) {
                if (params.hasOwnProperty(key)) {
                    const val = params[key] as string;
                    httpParams = httpParams.set(key, val);
                }
            }
            const result = await this.http.get<PaginatedResult>(
                `${this.baseUrl}/${id}/data`,
                { params: httpParams }
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取数据源的数据出错！' }
            );
            return {};
        }
    }

    public getPreviewUrl(id: string, type: PreviewType): string {
        if (type !== 'mapserver') {
            let url = `${this.baseUrl}/${id}/${type}`;
            if (url.startsWith('/')) {
                url = `${location.protocol}//${location.host}${url}`;
            }
            return url;
        }
        let url2 = this.apiRoot.substring(0, this.apiRoot.length - 3);
        if (url2.startsWith('/')) {
            url2 = `${location.protocol}//${location.host}${url2}`;
        }
        url2 = `${url2}rest/services/maps/${id}/MapServer/0`;
        return url2;
    }

    public async getCount(id: string, params: CountParam): Promise<number> {
        try {
            let httpParams = new HttpParams();
            for (const key in params) {
                if (params.hasOwnProperty(key)) {
                    const val = params[key] as string;
                    httpParams = httpParams.set(key, val);
                }
            }
            const result = await this.http.get<number>(
                `${this.baseUrl}/${id}/count`,
                { params: httpParams }
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取数据源的记录数出错！' }
            );
            return -1;
        }
    }

    public async getGeoJson(
        id: string,
        params: GeoJsonParam
    ): Promise<GeoJSON.FeatureCollection | undefined> {
        try {
            let httpParams = new HttpParams();
            for (const key in params) {
                if (params.hasOwnProperty(key)) {
                    const val = params[key] as string;
                    httpParams = httpParams.set(key, val);
                }
            }
            const result = await this.http.get<GeoJSON.FeatureCollection>(
                `${this.baseUrl}/${id}/geojson`,
                { params: httpParams,  }
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取数据源的记录数出错！' }
            );
            return undefined;
        }
    }

}

export interface CountParam {
    [key: string]: undefined | number | string;
    $where?: string;
}
export interface DistinctParam extends CountParam {
    $select?: string;
    $orderBy?: string;
}
export interface GeoJsonParam extends DistinctParam {
    $skip?: number;
    $take?: number;
}
export interface AgsJsonParam extends DistinctParam {
    $skip?: number;
    $take?: number;
}
export interface ReadDataParam extends DistinctParam {
    $groupBy?: string;
    $skip?: number;
    $take?: number;
}
export interface PivotParam extends DistinctParam {
    $aggregate?: string;
    $field?: string;
    $value?: string;
}
export interface PaginatedResult {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: any[];
}

/** 数据源（数据表或视图） */
export interface DataSourceModel {
    /** 数据源id */
    id: string;
    /** 数据源名称 */
    name?: string;
    /** 数据库连接 */
    connection?: { id?: string; name?: string };
    /** 数据表/视图架构 */
    schema?: string;
    /** 数据表/视图名称 */
    tableName?: string;
    /** 主键列名称 */
    primaryKeyColumn?: string;
    /** 显示列名称， 查询时不指定字段则返回数据表的主键列和显示列。 */
    displayColumn?: string;
    /** 空间列 */
    geometryColumn?: string;
    /** 预置过滤条件 */
    presetCriteria?: string;
    /** 默认排序 */
    defaultOrder?: string;
    /** 标签 */
    tags?: string[];
    /** 是否删除 */
    isDeleted?: boolean;
}

/** 数据源（数据表或视图） 搜索参数 */
export interface DataSourceSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
    /** 数据源名称或者数据表关键字 */
    keywords?: string;
}

/** 数据源（数据表或视图） 搜索结果 */
export interface DataSourceResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: DataSourceModel[];
}

export type PreviewType = 'data' | 'geojson' | 'featureset' | 'mapserver';
