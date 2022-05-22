import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AccountService } from 'app-shared';

import { DataSourceService } from '../datasources.service';

@Component({
    selector: 'app-datasources-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: DataSourceService
    ) { }

    public ngOnInit(): void {
        this.loadData();
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

    public resetSearch(): void {
        this.vm.searchModel.keywords = '';
        this.vm.searchModel.skip = 0;
        void this.vm.search();
    }

}
