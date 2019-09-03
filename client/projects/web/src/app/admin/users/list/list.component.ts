import { Component, OnInit } from '@angular/core';

import { UsersService } from '../users.service';

@Component({
    selector: 'app-admin-users-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    constructor(
        public vm: UsersService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.vm.search();
    }

}
