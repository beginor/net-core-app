import { Component, OnInit } from '@angular/core';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';

import { AppStorageService } from '../storages.service';
import { DetailComponent } from '../detail/detail.component';

@Component({
    selector: 'app-storage-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {

    constructor(
        private offcanvas: NgbOffcanvas,
        public account: AccountService,
        public vm: AppStorageService
    ) { }

    public ngOnInit(): void {
        void this.loadData();
    }

    public loadData(): void {
        void this.vm.search();
    }

    public showDetail(id: string, editable: boolean): void {
        const ref = this.offcanvas.open(
            DetailComponent,
            { position: 'end', panelClass: 'offcanvas-vw-40' }
        );
        const detail = ref.componentInstance as DetailComponent;
        detail.editable = editable;
        detail.id = id;
        void ref.result.then(() => {
            void this.vm.search();
        });
    }

    public delete(id: string): void {
        void this.vm.delete(id).then(deleted => {
            if (deleted) {
                void this.vm.search();
            }
        });
    }

    public onKeywordsChanged(): void {
        this.vm.searchModel.skip = 0;
        void this.vm.search();
    }

    public clearKeywords(): void {
        this.vm.searchModel.keywords = '';
        void this.vm.search();
    }

}
