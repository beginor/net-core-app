import { ErrorHandler, Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { CategoryCountModel, ResourceService } from './resorces.service';
import { UiService } from './ui.service';

/** 数据类别服务 */
@Injectable({ providedIn: 'root' })
export class CategoryService {

    public data = new BehaviorSubject<CategoryModel[]>([]);
    public loading = false;
    public nodes = new BehaviorSubject<CategoryNode[]>([]);
    public currentNode = new BehaviorSubject<CategoryNode | undefined>(
        undefined
    );

    private baseUrl: string;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler,
        private resource: ResourceService
    ) {
        this.baseUrl = `${apiRoot}/categories`;
    }

    /** 获取全部数据类别 */
    public async getAll(resourceType?: string): Promise<void> {
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
                    children: [],
                    level: 0
                }));
            roots.forEach(x => this.findChildren(x, models));
            this.data.next(models);
            this.nodes.next(roots);
            await this.getResourceCount(resourceType);
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
                children: [],
                level: parent.level + 1
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

    public findParent(node: CategoryNode): CategoryNode | undefined {
        if (!node.parentId) {
            return undefined;
        }
        return this._findParentNode(this.nodes.getValue(), node);
    }

    private _findParentNode(
        nodes: CategoryNode[],
        child: CategoryNode
    ): CategoryNode | undefined {
        for (const node of nodes) {
            if (child.parentId === node.id) {
                return node;
            }
        }
        for (const node of nodes) {
            if (node.children.length > 0) {
                const parent = this._findParentNode(node.children, child);
                if (!!parent) {
                    return parent;
                }
            }
        }
        return undefined;
    }

    public async handleDrop(
        e: CdkDragDrop<CategoryNode[], CategoryNode[], CategoryNode>
    ): Promise<void> {
        const nodes = e.container.data;
        const currentNode = nodes[e.currentIndex];
        let sequence: number;
        if (e.currentIndex === 0) {
            sequence = currentNode.sequence / 2.0;
        }
        else if (e.currentIndex === nodes.length - 1) {
            sequence = currentNode.sequence * 2.0;
        }
        else {
            const previousNode = nodes[e.currentIndex - 1];
            sequence = (currentNode.sequence + previousNode.sequence) / 2;
        }
        e.item.data.sequence = sequence;
        const model = await this.update(
            e.item.data.id as string, e.item.data as CategoryModel
        );
        if (!!model) {
            moveItemInArray(nodes, e.previousIndex, e.currentIndex);
        }
    }
    
    public getFlattenNodes(nodes: CategoryNode[]): CategoryNode[] {
        const result: CategoryNode[] = [];
        this.flattenNodes(nodes, result);
        return result;
    }
    
    private async getResourceCount(type?: string): Promise<void> {
        const countModels = await this.resource.getResourceCount(type);
        this.updateResourceCount(
            this.nodes.getValue(),
            countModels
        )
    }
    
    private updateResourceCount(
        nodes: CategoryNode[],
        countModels: CategoryCountModel[]
    ): void {
        for (const node of nodes) {
            const countModel = countModels.find(c => c.categoryId === node.id);
            if (!!countModel) {
                node.resourceCount = countModel.count;
            }
            if (!!node.children && node.children.length > 0) {
                this.updateResourceCount(node.children, countModels);
            }
        }
    }

    private flattenNodes(
        source: CategoryNode[],
        target: CategoryNode[]
    ): void {
        for (const node of source) {
            target.push(node);
            if (!!node.children && node.children.length > 0) {
                this.flattenNodes(node.children, target);
            }
        }
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
    resourceCount?: number;
    level: number;
}