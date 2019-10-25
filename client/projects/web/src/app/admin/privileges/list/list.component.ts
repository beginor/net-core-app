import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AccountService } from 'services';
import { AppPrivilegeService } from '../privileges.service';

@Component({
    selector: 'app-privilege-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
})
export class ListComponent implements OnInit {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public account: AccountService,
        public vm: AppPrivilegeService
    ) { }

    public ngOnInit(): void {
        this.loadData();
    }

    public showDetail(id: string, editable: boolean): void {
        this.router.navigate(
            ['./', id, { editable: editable }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    public async loadData(): Promise<void> {
        await this.vm.getModules();
        await this.vm.search();
    }

    public async delete(id: string): Promise<void> {
        const deleted = await this.vm.delete(id);
        if (deleted) {
            this.vm.search();
        }
    }

}
