import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** 切片地图服务 */
@Injectable({
    providedIn: 'root'
})
export class TileMapService {

    public searchModel: TileMapSearchModel = {
        skip: 0,
        take: 10,
        keywords: ''
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<TileMapModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl = `${this.apiRoot}/tilemaps`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) { }

    /** 搜索切片地图 */
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
                this.http.get<TileMapResultModel>(this.baseUrl, { params })
            );
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
            this.ui.showAlert(
                { type: 'danger', message: '加载切片地图数据出错!'}
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

    /** 创建切片地图 */
    public async create(
        model: TileMapModel
    ): Promise<TileMapModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.post<TileMapModel>(this.baseUrl, model)
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建切片地图出错！' }
            );
            return;
        }
    }

    /** 获取指定的切片地图 */
    public async getById(id: string): Promise<TileMapModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<TileMapModel>(`${this.baseUrl}/${id}`)
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的切片地图出错！' }
            );
            return;
        }
    }

    /** 删除切片地图 */
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
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除切片地图出错！' }
            );
            return false;
        }
    }

    /** 更新切片地图 */
    public async update(
        id: string,
        model: TileMapModel
    ): Promise<TileMapModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<TileMapModel>(`${this.baseUrl}/${id}`, model)
            );
            return result;
        }
        catch (ex) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新切片地图出错！' }
            );
            return;
        }
    }

    public getTileLayerUrl(id: string): string {
        if (!id) {
            return '';
        }
        let url = this.apiRoot.substring(0, this.apiRoot.length - 3);
        if (url.startsWith('/')) {
            url = `${location.protocol}//${location.host}${url}`;
        }
        url = `${url}rest/services/tilemaps/${id}/MapServer`;
        return url;
    }

}

/** 切片地图 */
export interface TileMapModel {
    /** 切片地图id */
    id: string;
    /** 切片地图名称 */
    name?: string;
    /** 缓存目录 */
    cacheDirectory?: string;
    /** 切片信息路径 */
    mapTileInfoPath?: string;
    /** 目录结构 */
    folderStructure?: string;
    /** 内容类型 */
    contentType?: string;
    /** 是否为紧凑格式 */
    isBundled?: boolean;
    /** 最小缩放级别 */
    minLevel?: number;
    /** 最大缩放级别 */
    maxLevel?: number;
    /** 最小纬度 */
    minLatitude?: number;
    /** 最大纬度 */
    maxLatitude?: number;
    /** 最小经度 */
    minLongitude?: number;
    /** 最大经度 */
    maxLongitude?: number;
    /** 创建时间 */
    createdAt?: string;
    /** 更新时间 */
    updatedAt?: string;
}

/** 切片地图 搜索参数 */
export interface TileMapSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
    keywords?: string;
}

/** 切片地图 搜索结果 */
export interface TileMapResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: TileMapModel[];
}
