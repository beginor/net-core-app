import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AccountService } from 'app-shared';

import { CategoryService } from '../categories.service';

@Component({
    selector: 'app-category-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: CategoryService
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

}