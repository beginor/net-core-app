import { Component, OnInit } from '@angular/core';

import { AppPrivilegeService } from '../privileges.service';

@Component({
    selector: 'app-privilege-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

constructor(
        public vm: AppPrivilegeService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.vm.search();
    }

    public async onPageChange(p: number): Promise<void> {
        this.vm.searchModel.skip = (p - 1) * this.vm.searchModel.take;
        await this.vm.search();
    }

    public async onPageSizeChange(e: Event): Promise<void> {
        const el = e.target as HTMLSelectElement;
        const pageSize = parseInt(el.value, 10);
        this.vm.searchModel.take = pageSize;
        this.vm.searchModel.skip = 0;
        await this.vm.search();
    }

}
