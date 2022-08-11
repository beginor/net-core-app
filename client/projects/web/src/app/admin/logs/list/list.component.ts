import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';

import { AppLogService } from '../logs.service';

@Component({
    selector: 'app-log-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
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
        void this.router.navigate(
            ['./', id, { editable: editable }],
            { relativeTo: this.route, skipLocationChange: true }
        );
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
