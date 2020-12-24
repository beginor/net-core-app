import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { UiService } from 'projects/web/src/app/common';

@Injectable({ providedIn: 'root' })
export class MetadataService {

    private url = `${this.apiRoot}/metadata`;

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
        private ui: UiService
    ) { }

    public async getStatus(id: string): Promise<void> {
        await this.http.get(
            `${this.url}/${id}/status`
        ).toPromise();
    }

    public async getSchemas(id: string): Promise<string[]> {
        const schemas = await this.http.get<string[]>(
            `${this.url}/${id}/schemas`
        ).toPromise();
        return schemas;
    }

    public async getTables(id: string, schema: string): Promise<TableModel[]> {
        const params = new HttpParams()
            .set('schema', schema);
        const tables = await this.http.get<TableModel[]>(
            `${this.url}/${id}/tables`,
            { params }
        ).toPromise();
        return tables;
    }

    public async GetColumns(
        id: string,
        schema: string,
        tableName: string
    ): Promise<ColumnModel[]> {
        const params = new HttpParams()
            .set('schema', schema);
        const columns = await this.http.get<ColumnModel[]>(
            `${this.url}/${id}/tables/${tableName}/columns`,
            { params }
        ).toPromise();
        return columns;
    }

}

export interface TableModel {
    tableSchema?: string;
    tableName: string;
    description?: string;
    tableType?: string;
}

export interface ColumnModel {
    tableSchema?: string;
    tableName?: string;
    columnName?: string;
    description?: string;
    dataType?: string;
    length?: number;
    isNullable?: boolean;
}
