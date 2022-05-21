import { Injectable, Inject, ErrorHandler } from '@angular/core';
import {
    HttpClient, HttpEventType, HttpParams, HttpRequest
} from '@angular/common/http';

import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';
import { RolesService, AppRoleModel } from '../../admin/roles/roles.service';

/** 数据服务服务 */
@Injectable({
    providedIn: 'root'
})
export class DataServiceService {

    public searchModel: DataServiceSearchModel = { skip: 0, take: 10 };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<DataServiceModel[]>([]);
    public loading = false;
    public showPagination = false;
    public roles: AppRoleModel[] = [];

    private baseUrl = `${this.apiRoot}/dataservices`;
    private rolesSvc: RolesService;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) {
        this.rolesSvc = new RolesService(http, apiRoot, ui, errorHandler);
        this.rolesSvc.data.subscribe(data => { this.roles = data; });
    }

    /** 搜索数据服务 */
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
            const result = await lastValueFrom(
                this.http.get<DataServiceResultModel>(this.baseUrl, { params })
            );
            const total = result.total ?? 0;
            const data = result.data ?? [];
            this.total.next(total);
            this.data.next(data);
            this.showPagination = total > data.length;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.total.next(0);
            this.data.next([]);
            this.ui.showAlert(
                { type: 'danger', message: '加载数据服务数据出错!'}
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

    /** 创建数据服务 */
    public async create(
        model: DataServiceModel
    ): Promise<DataServiceModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.post<DataServiceModel>(this.baseUrl, model)
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建数据服务出错！' }
            );
            return;
        }
    }

    /** 获取指定的数据服务 */
    public async getById(id: string): Promise<DataServiceModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<DataServiceModel>(`${this.baseUrl}/${id}`)
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的数据服务出错！' }
            );
            return;
        }
    }

    /** 删除数据服务 */
    public async delete(id: string): Promise<boolean> {
        const confirm = await this.ui.showConfirm('确认删除么？');
        if (!confirm) {
            return false;
        }
        try {
            await lastValueFrom(
                this.http.delete(`${this.baseUrl}/${id}`)
            );
            return true;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除数据服务出错！' }
            );
            return false;
        }
    }

    /** 更新数据服务 */
    public async update(
        id: string,
        model: DataServiceModel
    ): Promise<DataServiceModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<DataServiceModel>(`${this.baseUrl}/${id}`, model)
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新数据服务出错！' }
            );
            return;
        }
    }

    public async getColumns(id: string): Promise<DataServiceFieldModel[]> {
        try {
            const result = await lastValueFrom(
                this.http.get<DataServiceFieldModel[]>(`${this.baseUrl}/${id}/columns`) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取数据服务的列数据出错！' }
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
            const result = await lastValueFrom(
                this.http.get<PaginatedResult>(`${this.baseUrl}/${id}/data`, { params: httpParams }) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取数据服务的数据出错！' }
            );
            return {};
        }
    }

    public getPreviewUrl(id: string, type: PreviewType): string {
        let url = `${this.baseUrl}/${id}/${type}`;
        if (url.startsWith('/')) {
            url = `${location.protocol}//${location.host}${url}`;
        }
        if (type === 'geojson' || type === 'featureset' || type === 'data') {
            return url;
        }
        if (type === 'mvt') {
            url += '/{z}/{y}/{x}';
            return url;
        }
        let url2 = this.apiRoot.substring(0, this.apiRoot.length - 3);
        if (url2.startsWith('/')) {
            url2 = `${location.protocol}//${location.host}${url2}`;
        }
        url2 = `${url2}rest/services/features/${id}/MapServer/0`;
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
            const result = await lastValueFrom(
                this.http.get<number>(`${this.baseUrl}/${id}/count`, { params: httpParams }) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取数据服务的记录数出错！' }
            );
            return -1;
        }
    }

    public getGeoJson(
        id: string,
        params: GeoJsonParam,
        progressCallback: (total: number, loaded: number) => void
    ): Promise<GeoJSON.FeatureCollection> {
        return this.getDataWithProgressCallback<GeoJSON.FeatureCollection>(
            `${this.baseUrl}/${id}/geojson`,
            params,
            progressCallback
        );
    }

    public getFeatureSetJson(
        id: string,
        params: AgsJsonParam,
        progressCallback: (total: number, loaded: number) => void
    ): Promise<__esri.FeatureSetProperties> {
        return this.getDataWithProgressCallback<__esri.FeatureSetProperties>(
            `${this.baseUrl}/${id}/featureset`,
            params,
            progressCallback
        );
    }

    private getDataWithProgressCallback<T>(
        url: string,
        params: any,
        progressCallback: (total: number, loaded: number) => void
    ): Promise<T> {
        return new Promise<T>((resolve, reject) => {
            let httpParams = new HttpParams();
            for (const key in params) {
                if (params.hasOwnProperty(key)) {
                    const val = params[key] as string;
                    httpParams = httpParams.set(key, val);
                }
            }
            const req = new HttpRequest(
                'GET',
                url,
                { params: httpParams, reportProgress: true }
            );
            this.http.request<T>(req).subscribe({
                next: e => {
                    if (e.type === HttpEventType.DownloadProgress) {
                        progressCallback(e.total || 0, e.loaded);
                    }
                    else if (e.type === HttpEventType.Response) {
                        resolve(e.body as T);
                    }
                },
                error: ex => {
                    this.errorHandler.handleError(ex);
                    this.ui.showAlert(
                        { type: 'danger', message: '获取数据服务的数据出错！' }
                    );
                    reject(ex);
                }
            });
        });
    }

    public async getAllRoles(): Promise<void> {
        try {
            this.rolesSvc.searchModel.skip = 0;
            this.rolesSvc.searchModel.take = 999;
            await this.rolesSvc.search();
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取角色列表出错！' }
            );
        }
    }

    public async supportMvt(id: string): Promise<boolean> {
        try {
            const result = await lastValueFrom(this.http.get<boolean>(`${this.baseUrl}/${id}/mvt/support`)); // eslint-disable-line max-len
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            return false;
        }
    }

    public async getMvtInfo(id: string): Promise<MvtInfo | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<MvtInfo>(`${this.baseUrl}/${id}/mvt/info`)
            );
            return result;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert({
                type: 'danger',
                message: '获取图层的矢量切片信息出错！'
            });
            return;
        }
    }

    public async getMvtCache(id: string): Promise<number> {
        try {
            const cacheSize = await lastValueFrom(
                this.http.get<number>(`${this.baseUrl}/${id}/mvt/cache`)
            );
            return cacheSize;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取图层的矢量切片缓存信息出错！' }
            );
            return 0;
        }
    }

    public async deleteMvtCache(id: string): Promise<boolean> {
        const confirm = await this.ui.showConfirm('确认删除矢量切片缓存么？');
        if (!confirm) {
            return false;
        }
        try {
            await lastValueFrom(
                this.http.delete(`${this.baseUrl}/${id}/mvt/cache`)
            );
            return true;
        }
        catch (ex: any) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除数据服务出错！' }
            );
            return false;
        }
    }
}

export interface MvtInfo {
    layerName: string;
    description: string;
    geometryType: string;
    minzoom: number;
    maxzoom: number;
    bounds: number[];
    url: string;
}

export interface CountParam {
    [key: string]: undefined | number | string | boolean;
    $where?: string;
}
export interface DistinctParam extends CountParam {
    $select?: string;
    $orderBy?: string;
}
export interface GeoJsonParam extends DistinctParam {
    $skip?: number;
    $take?: number;
    $returnBbox?: boolean;
}
export interface AgsJsonParam extends DistinctParam {
    $skip?: number;
    $take?: number;
    $returnExtent?: boolean;
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

/** 数据服务 */
export interface DataServiceModel {
    /** 数据服务id */
    id: string;
    /** 数据服务名称 */
    name?: string;
    /** 数据服务描述 */
    description?: string;
    /** 类别 */
    category?: { id?: string; name?: string };
    /** 允许的角色 */
    roles?: string[];
    /** 标签 */
    tags?: string[];
    /** 数据源 */
    dataSource?: { id?: string; name?: string };
    /** 数据表/视图架构 */
    schema?: string;
    /** 数据表/视图名称 */
    tableName?: string;
    /** 数据服务的公开字段 */
    fields?: DataServiceFieldModel[];
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
    /** 支持矢量切片 */
    supportMvt?: boolean;
    /** 矢量切片缓存时间 */
    mvtCacheDuration?: number;
    /** 矢量切片最大级别 */
    mvtMaxZoom?: number;
    /** 矢量切片最小级别 */
    mvtMinZoom?: number;
}

export interface DataServiceFieldModel {
    name: string;
    description?: string;
    type?: string;
    length?: number;
    nullable?: boolean;
    editable?: boolean;
}

/** 数据服务 搜索参数 */
export interface DataServiceSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
    /** 数据服务名称或者数据表关键字 */
    keywords?: string;
    /** 类别 */
    category?: string;
}

/** 数据服务 搜索结果 */
export interface DataServiceResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: DataServiceModel[];
}

export type PreviewType = 'data' | 'geojson' | 'featureset' | 'mapserver' | 'mvt'; // eslint-disable-line max-len
