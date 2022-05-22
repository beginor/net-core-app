import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';
import { CategoryNode, CategoryTreeViewComponent } from '../../../common';

import { DataServiceModel, DataServiceService } from '../dataservices.service';
import { PreviewComponent } from '../preview/preview.component';

@Component({
    selector: 'app-dataservices-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {

    @ViewChild('categoryTreeView', { static: true })
    public categoryTreeView!: CategoryTreeViewComponent;

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private modal: NgbModal,
        public account: AccountService,
        public vm: DataServiceService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.categoryTreeView.loadData();
        await this.loadData();
    }

    public async loadData(): Promise<void> {
        await this.vm.search();
    }

    public showDetail(id: string, editable: boolean): void {
        void this.router.navigate(
            ['./', id, { editable: editable }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    public async delete(id: string): Promise<void> {
        const deleted = await this.vm.delete(id);
        if (deleted) {
            void this.vm.search();
        }
    }

    public resetSearch(): void {
        this.vm.searchModel.keywords = '';
        this.vm.searchModel.skip = 0;
        void this.vm.search();
    }

    public showPreview(ds: DataServiceModel): void {
        const ref = this.modal.open(
            PreviewComponent,
            {
                container: 'body',
                size: 'xl',
                keyboard: false,
                backdrop: 'static'
            }
        );
        Object.assign(ref.componentInstance, { ds });
        // ref.componentInstance.ds = ds;
    }

    public onTreeItemClick(node: CategoryNode): void {
        if (!node) {
            delete this.vm.searchModel.category;
        }
        else {
            this.vm.searchModel.category = node.id;
        }
        void this.loadData();
    }

}
