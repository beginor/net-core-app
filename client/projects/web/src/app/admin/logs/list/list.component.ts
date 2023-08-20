import { Component, OnInit } from '@angular/core';
import { NgbDate, NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';

import { AppLogService } from '../logs.service';
import { DetailComponent } from '../detail/detail.component';

@Component({
    selector: 'app-log-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {

    constructor(
        private offcanvas: NgbOffcanvas,
        public account: AccountService,
        public vm: AppLogService
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
        }).catch(ex => {
            console.log(`offcanvas canceled with reason ${ex}`)
        });
    }

    public onSelectDate(d: NgbDate): void {
        // this.vm.searchDate = d;
        this.vm.searchModel.skip = 0;
        void this.vm.search();
    }

    public onSelectLevel(): void {
        this.vm.searchModel.skip = 0;
        void this.vm.search();
    }

}
