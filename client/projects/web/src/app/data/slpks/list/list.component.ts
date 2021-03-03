import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';
import { PreviewComponent } from '../preview/preview.component';
import { SlpkModel, SlpkService } from '../slpks.service';

@Component({
    selector: 'app-slpk-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private modal: NgbModal,
        public account: AccountService,
        public vm: SlpkService
    ) { }

    public ngOnInit(): void {
        this.loadData();
    }

    public async loadData(): Promise<void> {
        await this.vm.search();
    }

    public showDetail(id: string, editable: boolean): void {
        this.router.navigate(
            ['./', id, { editable: editable }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    public async delete(id: string): Promise<void> {
        const deleted = await this.vm.delete(id);
        if (deleted) {
            this.vm.search();
        }
    }

    public async resetSearch(): Promise<void> {
        this.vm.searchModel.keywords = '';
        this.vm.searchModel.skip = 0;
        this.vm.search();
    }

    public showPreview(item: SlpkModel): void {
        const modalRef = this.modal.open(
            PreviewComponent,
            { container: 'body', size: 'xl' }
        );
        const id = item.id;
        let tag = '';
        if (!!item.tags && !!item.tags[0]) {
            tag = item.tags[0];
        }
        modalRef.componentInstance.id = id;
        modalRef.componentInstance.name = tag;
    }

}
