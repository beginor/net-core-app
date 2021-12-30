import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';

import { DataApiService, DataApiModel } from '../dataapis.service';
import { PreviewComponent } from '../preview/preview.component';

@Component({
    selector: 'app-dataapi-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        private modal: NgbModal,
        public vm: DataApiService
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
        Object.assign(ref.componentInstance, { id: api.id, title: api.name });
    }

    public resetSearch(): void {
        this.vm.searchModel.keywords = '';
        this.vm.searchModel.skip = 0;
        void this.vm.search();
    }

}
