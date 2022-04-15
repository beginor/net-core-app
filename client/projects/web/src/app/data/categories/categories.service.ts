import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

/** 数据类别服务 */
@Injectable({
    providedIn: 'root'
})
export class CategoryService {

    public data = new BehaviorSubject<CategoryModel[]>([]);
    public loading = false;
    public nodes = new BehaviorSubject<CategoryNode[]>([]);

    private baseUrl = `${this.apiRoot}/categories`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) { }

    /** 获取全部数据类别 */
    public async getAll(): Promise<void> {
        this.loading = true;
        try {
            const models = await lastValueFrom(
                this.http.get<CategoryModel[]>(this.baseUrl) // eslint-disable-line max-len
            );
            const roots = models.filter(x => !x.parentId)
                .map<CategoryNode>(x => ({
                    id: x.id,
                    name: x.name,
                    parentId: x.parentId,
                    sequence: x.sequence,
                    children: []
                }));
            roots.forEach(x => this.findChildren(x, models));
            this.data.next(models);
            this.nodes.next(roots);
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.data.next([]);
            this.ui.showAlert(
                { type: 'danger', message: '加载数据类别出错!'}
            );
        }
        finally {
            this.loading = false;
        }
    }

    private findChildren(parent: CategoryNode, models: CategoryModel[]): void {
        parent.children = models.filter(x => x.parentId === parent.id)
            .map<CategoryNode>(x => ({
                id: x.id,
                name: x.name,
                parentId: x.parentId,
                sequence: x.sequence,
                children: []
            }));
        if (parent.children.length > 0) {
            parent.children.forEach(x => {
                this.findChildren(x, models);
            });
        }
    };

    /** 创建数据类别 */
    public async create(
        model: CategoryModel
    ): Promise<CategoryModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.post<CategoryModel>(this.baseUrl, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建数据类别出错！' }
            );
            return;
        }
    }

    /** 获取指定的数据类别 */
    public async getById(id: string): Promise<CategoryModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<CategoryModel>(`${this.baseUrl}/${id}`) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的数据类别出错！' }
            );
            return;
        }
    }

    /** 删除数据类别 */
    public async delete(id: string): Promise<boolean> {
        const confirm = await this.ui.showConfirm('确认删除么？');
        if (!confirm) {
            return false;
        }
        try {
            await lastValueFrom(
                this.http.delete(`${this.baseUrl}/${id}`) // eslint-disable-line max-len
            );
            return true;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除数据类别出错！' }
            );
            return false;
        }
    }

    /** 更新数据类别 */
    public async update(
        id: string,
        model: CategoryModel
    ): Promise<CategoryModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<CategoryModel>(`${this.baseUrl}/${id}`, model) // eslint-disable-line max-len
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新数据类别出错！' }
            );
            return;
        }
    }

    public createEmptyNode(): CategoryNode {
        return { children: [] } as unknown as CategoryNode;
    }

}

/** 数据类别 */
export interface CategoryModel {
    /** 类别ID */
    id: string;
    /** 类别名称 */
    name: string;
    /** 父类ID */
    parentId?: string;
    /** 顺序号 */
    sequence: number;
}

export interface CategoryNode {
    id?: string;
    name: string;
    parentId?: string;
    sequence: number;
    children: CategoryNode[];
}
