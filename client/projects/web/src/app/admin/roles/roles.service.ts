import { Injectable, Inject, ErrorHandler } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

import {
    AppPrivilegeService, AppPrivilegeModel
} from '../privileges/privileges.service';

/** 角色服务 */
@Injectable({
    providedIn: 'root'
})
export class RolesService {

    public searchModel: AppRoleSearchModel = {
        skip: 0,
        take: 10
    };
    public total = new BehaviorSubject<number>(0);
    public data = new BehaviorSubject<AppRoleModel[]>([]);
    public loading = false;
    public showPagination = false;
    public privileges: ModulePrivileges[] = [];
    public rolePrivileges: { [key: string]: boolean } = {};

    private baseUrl = `${this.apiRoot}/roles`;
    private privilegeService: AppPrivilegeService;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService,
        private errorHandler: ErrorHandler
    ) {
        this.privilegeService = new AppPrivilegeService(
            http,
            apiRoot,
            ui,
            errorHandler
        );
        this.privilegeService.data.subscribe(data => {
            this.privileges = [];
            for (const privilege of data) {
                let mp = this.privileges.find(
                    m => m.module === privilege.module
                );
                if (!mp) {
                    mp = { module: privilege.module ?? '', privileges: [] };
                    this.privileges.push(mp);
                }
                mp.privileges.push(privilege);
            }
        });
    }

    /** 搜索角色 */
    public async search(): Promise<void> {
        let params = new HttpParams();
        for (const key in this.searchModel) {
            if (this.searchModel.hasOwnProperty(key)) {
                const val = this.searchModel[key];
                params = params.set(key, val as string);
            }
        }
        this.loading = true;
        try {
            const result = await lastValueFrom(
                this.http.get<AppRoleResultModel>(this.baseUrl, { params })
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
                { type: 'danger', message: '加载角色数据出错!'}
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

    /** 创建角色 */
    public async create(
        model: AppRoleModel
    ): Promise<AppRoleModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.post<AppRoleModel>(this.baseUrl, model)
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '创建角色出错！' }
            );
            return;
        }
    }

    /** 获取指定的角色 */
    public async getById(id: string): Promise<AppRoleModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.get<AppRoleModel>(`${this.baseUrl}/${id}`)
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取指定的角色出错！' }
            );
            return;
        }
    }

    /** 删除角色 */
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
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '删除角色出错！' }
            );
            return false;
        }
    }

    /** 更新角色 */
    public async update(
        id: string,
        model: AppRoleModel
    ): Promise<AppRoleModel | undefined> {
        try {
            const result = await lastValueFrom(
                this.http.put<AppRoleModel>(`${this.baseUrl}/${id}`, model)
            );
            return result;
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '更新角色出错！' }
            );
            return;
        }
    }

    public async getAllPrivileges(): Promise<void> {
        try {
            if (this.privileges.length > 0) {
                return;
            }
            this.privilegeService.searchModel.skip = 0;
            this.privilegeService.searchModel.take = 999;
            this.privilegeService.searchModel.module = '';
            await this.privilegeService.search();
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert({ type: 'danger', message: '获取全部权限出错！' });
        }
    }

    public async getPrivilegesForRole(roleId: string): Promise<void> {
        try {
            this.rolePrivileges = {};
            const url = `${this.baseUrl}/${roleId}/privileges`;
            const privileges = await lastValueFrom(
                this.http.get<string[]>(url)
            );
            for (const p of privileges) {
                this.rolePrivileges[p] = true;
            }
        }
        catch (ex: unknown) {
            this.errorHandler.handleError(ex);
            this.ui.showAlert(
                { type: 'danger', message: '获取角色权限出错！' }
            );
        }
    }

    public cleanUp(): void {
        this.privileges = [];
    }

    public async toggleRolePrivilege(
        roleId: string,
        privilege: string
    ): Promise<void> {
        const url = `${this.baseUrl}/${roleId}/privileges/${privilege}`;
        if (!!this.rolePrivileges[privilege]) {
            try {
                // remove privilege from role;
                await lastValueFrom(this.http.delete(url));
                this.rolePrivileges[privilege] = false;
            }
            catch (ex: unknown) {
                this.errorHandler.handleError(ex);
                this.ui.showAlert(
                    { type: 'danger', message: '删除角色权限失败！' }
                );
                this.rolePrivileges[privilege] = true;
            }
        }
        else {
            try {
                // add privilege to role;
                await lastValueFrom(this.http.post(url, null));
                this.rolePrivileges[privilege] = true;
            }
            catch (ex: unknown) {
                this.errorHandler.handleError(ex);
                this.ui.showAlert(
                    { type: 'danger', message: '添加角色权限失败！' }
                );
                this.rolePrivileges[privilege] = false;
            }
        }
    }

}

/** 角色 */
export interface AppRoleModel {
    /** 角色标识 */
    id: string;
    /** 角色名称 */
    name: string;
    /** 角色描述 */
    description?: string;
    /** 是否默认 */
    isDefault?: boolean;
    /** 是否匿名 */
    isAnonymous?: boolean;
    /** 用户数量 */
    userCount?: number;
}

/** 角色 搜索参数 */
export interface AppRoleSearchModel {
    [key: string]: undefined | number | string;
    /** 跳过的记录数 */
    skip: number;
    /** 取多少条记录 */
    take: number;
}

/** 角色 搜索结果 */
export interface AppRoleResultModel {
    /** 请求跳过的记录数 */
    skip?: number;
    /** 请求多少条记录 */
    take?: number;
    /** 总记录数 */
    total?: number;
    /** 数据列表 */
    data?: AppRoleModel[];
}

export interface ModulePrivileges {
    module: string;
    privileges: AppPrivilegeModel[];
}
