import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** 矢量切片包服务 */
@Injectable({
    providedIn: 'root'
})
export class VectortileService {

    public searchModel: VectortileSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<VectortileModel[]>([]);
    public loading = false;
    public showPagination = false;

    private baseUrl = `${this.apiRoot}/vectortiles`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService
    ) { }

    /** 搜索矢量切片包 */
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
            const result = await this.http.get<VectortileResultModel>(
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
                { type: 'danger', message: '加载矢量切片包数据出错!'}
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

    /** 创建矢量切片包 */
    public async create(
        model: VectortileModel
    ): Promise<VectortileModel | undefined> {
        try {
            const result = await this.http.post<VectortileModel>(
                this.baseUrl,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建矢量切片包出错！' }
            );
            return;
        }
    }

    /** 获取指定的矢量切片包 */
    public async getById(id: string): Promise<VectortileModel | undefined> {
        try {
            const result = await this.http.get<VectortileModel>(
                `${this.baseUrl}/${id}`
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的矢量切片包出错！' }
            );
            return;
        }
    }

    /** 删除矢量切片包 */
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
                { type: 'danger', message: '删除矢量切片包出错！' }
            );
            return false;
        }
    }

    /** 更新矢量切片包 */
    public async update(
        id: string,
        model: VectortileModel
    ): Promise<VectortileModel | undefined> {
        try {
            const result = await this.http.put<VectortileModel>(
                `${this.baseUrl}/${id}`,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新矢量切片包出错！' }
            );
            return;
        }
    }

    public getVectorTileLayerUrl(id: string): string {
        if (!id) {
            return '';
        }
        let url = this.apiRoot.substring(0, this.apiRoot.length - 3);
        if (url.startsWith('/')) {
            url = `${location.protocol}//${location.host}${url}`;
        }
        url = `${url}rest/services/vectortiles/${id}/VectorTileServer`;
        return url;
    }

    public getVectorTileAssetsUrl(): string {
        let url = this.apiRoot.substring(0, this.apiRoot.length - 3);
        if (url.startsWith('/')) {
            url = `${location.protocol}//${location.host}${url}`;
        }
        url = `${url}web/assets/mapbox/`;
        return url;
    }

}

/** 矢量切片包 */
export interface VectortileModel {
    /** 矢量切片包ID */
    id: string;
    /** 矢量切片包名称 */
    name?: string;
    /** 矢量切片包目录 */
    directory?: string;
    /** 最小缩放级别 */
    minZoom?: number;
    /** 最大缩放级别 */
    maxZoom?: number;
    /** 默认样式 */
    defaultStyle?: string;
    /** 样式内容 */
    styleContent?: string;
}

/** 矢量切片包 搜索参数 */
export interface VectortileSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
    /** 查询关键字 */
    keywords?: string;
}

/** 矢量切片包 搜索结果 */
export interface VectortileResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: VectortileModel[];
}
