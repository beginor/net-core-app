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
        await this.vm.getModules();
        await this.vm.search();
    }

}
