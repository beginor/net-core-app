import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** 导航节点（菜单）服务 */
@Injectable({
    providedIn: 'root'
})
export class NavItemsService {

    public searchModel: AppNavItemSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<NavItemModel[]>([]);
    public loading: boolean;

    private baseUrl = `${this.apiRoot}/app-nav-items`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService
    ) { }

    /** 搜索导航节点（菜单） */
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
            const result = await this.http.get<AppNavItemResultModel>(
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
            this.ui.showAlert({ type: 'danger', message: '加载导航节点（菜单）数据出错!'});
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

    /** 创建导航节点（菜单） */
    public async create(model: NavItemModel): Promise<NavItemModel> {
        try {
            const result = await this.http.post<NavItemModel>(
                this.baseUrl,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert({ type: 'danger', message: '创建导航节点（菜单）出错！' });
            return null;
        }
    }

    /** 获取指定的导航节点（菜单） */
    public async getById(id: string): Promise<NavItemModel> {
        try {
            const result = await this.http.get<NavItemModel>(
                `${this.baseUrl}/${id}`
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert({ type: 'danger', message: '获取指定的导航节点（菜单）出错！' });
            return null;
        }
    }

    /** 删除导航节点（菜单） */
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
            this.ui.showAlert({ type: 'danger', message: '删除导航节点（菜单）出错！' });
            return false;
        }
    }

    /** 更新导航节点（菜单） */
    public async update(
        id: string,
        model: NavItemModel
    ): Promise<NavItemModel> {
        try {
            const result = await this.http.put<NavItemModel>(
                `${this.baseUrl}/${id}`,
                model
            ).toPromise();
            return result;
        }
        catch (ex) {
            console.error(ex);
            this.ui.showAlert({ type: 'danger', message: '更新导航节点（菜单）出错！' });
            return null;
        }
    }

}

/** 导航节点（菜单） */
export interface NavItemModel {
    /** 节点ID */
    id?: string;
    /** parent_id, int8 */
    parentId?: string;
    /** 标题 */
    title?: string;
    /** 提示文字 */
    tooltip?: string;
    /** 图标 */
    icon?: string;
    /** 导航地址 */
    url?: string;
    /** 顺序 */
    sequence?: number;
}

/** 导航节点（菜单） 搜索参数 */
export interface AppNavItemSearchModel {
    /** 跳过的记录数 */
    skip?: number;
    /** 取多少条记录 */
    take?: number;
}

/** 导航节点（菜单） 搜索结果 */
export interface AppNavItemResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: NavItemModel[];
}
