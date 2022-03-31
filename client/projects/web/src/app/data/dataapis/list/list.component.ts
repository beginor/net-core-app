import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';

import { DataApiService, DataApiModel } from '../dataapis.service';
import { PreviewComponent } from '../preview/preview.component';
import { ExportComponent } from '../export/export.component';

@Component({
    selector: 'app-dataapi-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    public exportingApiDoc = false;
    public selectedApis: string[] = [];

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private modal: NgbModal,
        public account: AccountService,
        public vm: DataApiService,
    ) { }

    public ngOnInit(): void {
        void this.loadData();
    }

    public loadData(): void {
        void this.vm.search();
    }

    public showDetail(id: string, editable: boolean): void {
        void this.router.navigate(
            ['./', id, { editable: editable }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    public delete(id: string): void {
        void this.vm.delete(id).then(deleted => {
            if (deleted) {
                void this.vm.search();
            }
        });
    }

    public showStatement(id: string): void {
        void this.router.navigate(
            ['./', id, 'statement'],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    public showPreview(api: DataApiModel): void {
        const ref = this.modal.open(
            PreviewComponent,
            {
                container: 'body',
                size: 'xl',
                keyboard: false,
                backdrop: 'static'
            }
        );
        Object.assign(
            ref.componentInstance,
            { id: api.id, title: api.name, hasGeoColumn: !!api.geometryColumn }
        );
    }

    public resetSearch(): void {
        this.vm.searchModel.keywords = '';
        this.vm.searchModel.skip = 0;
        void this.vm.search();
    }

    public isSelected(apiId: string): boolean {
        return this.selectedApis.indexOf(apiId) !== -1;
    }

    public toggleSelected(apiId: string): void {
        const idx = this.selectedApis.indexOf(apiId);
        if (idx === -1) {
            this.selectedApis.push(apiId);
        }
        else {
            this.selectedApis.splice(idx, 1);
        }
    }

    public isAllSelected(): boolean {
        return this.selectedApis.length == this.vm.data.getValue().length;
    }

    public toggleSelectAll(): void {
        if (this.selectedApis.length < this.vm.data.getValue().length) {
            this.selectedApis = this.vm.data.getValue().map(item => item.id);
        }
        else {
            this.selectedApis = [];
        }
    }

    public async exportApiDoc(): Promise<void> {
        const ref = this.modal.open(ExportComponent, {
            container: 'body',
            size: 'md',
            keyboard: false,
            backdrop: 'static',
            scrollable: true
        });
        const comp = ref.componentInstance as ExportComponent;
        comp.model.apis = this.selectedApis;
        void ref.result.then(_ => {
            this.selectedApis = [];
            this.exportingApiDoc = false;
        });
    };

}
