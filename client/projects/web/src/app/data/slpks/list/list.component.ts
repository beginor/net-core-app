import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';
import { CategoryNode, CategoryTreeViewComponent } from "../../../common";
import { PreviewComponent } from '../preview/preview.component';
import { SlpkModel, SlpkService } from '../slpks.service';

@Component({
    selector: 'app-slpk-list',
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
        public vm: SlpkService
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

    public async resetSearch(): Promise<void> {
        this.vm.searchModel.keywords = '';
        this.vm.searchModel.skip = 0;
        await this.vm.search();
    }

    public showPreview(item: SlpkModel): void {
        const modalRef = this.modal.open(
            PreviewComponent,
            {
                container: 'body',
                size: 'xl',
                keyboard: false,
                backdrop: 'static'
            }
        );
        const id = item.id;
        const name = item.name;
        Object.assign(modalRef.componentInstance, {id, name });
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
