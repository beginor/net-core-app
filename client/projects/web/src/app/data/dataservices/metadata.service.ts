import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

import { lastValueFrom } from 'rxjs';

import { UiService } from 'projects/web/src/app/common';

@Injectable({ providedIn: 'root' })
export class MetadataService {

    private url: string;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService
    ) {
        this.url  = `${this.apiRoot}/metadata`;
    }

    public async getStatus(id: string): Promise<void> {
        await lastValueFrom(
            this.http.get(`${this.url}/${id}/status`)
        );
    }

    public async getSchemas(id: string): Promise<string[]> {
        const schemas = await lastValueFrom(
            this.http.get<string[]>(`${this.url}/${id}/schemas`)
        );
        return schemas;
    }

    public async getTables(id: string, schema?: string): Promise<TableModel[]> {
        let params = new HttpParams();
        if (!!schema) {
            params = params.set('schema', schema);
        }
        const tables = await lastValueFrom(
            this.http.get<TableModel[]>(`${this.url}/${id}/tables`, { params })
        );
        return tables;
    }

    public async getColumns(
        id: string,
        schema: string,
        tableName: string
    ): Promise<ColumnModel[]> {
        const params = new HttpParams()
            .set('schema', schema);
        const columns = await lastValueFrom(
            this.http.get<ColumnModel[]>(`${this.url}/${id}/tables/${tableName}/columns`, { params }) // eslint-disable-line max-len
        );
        return columns;
    }

}

export interface TableModel {
    schema?: string;
    name: string;
    description?: string;
    type?: string;
}

export interface ColumnModel {
    schema?: string;
    table?: string;
    name: string;
    description?: string;
    type?: string;
    length?: number;
    nullable?: boolean;
}
