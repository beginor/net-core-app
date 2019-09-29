import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AppPrivilegeService } from '../privileges.service';
import {
    trigger, transition, animate, keyframes, style
} from '@angular/animations';

@Component({
    selector: 'app-privilege-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
})
export class ListComponent implements OnInit {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        public vm: AppPrivilegeService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.vm.getModules();
        await this.vm.search();
    }

    public async showDetail(id: string, editable: boolean): Promise<void> {
        await this.router.navigate(
            ['./', id, { editable: editable }],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

}
