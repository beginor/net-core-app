import { Component, OnInit } from '@angular/core';

import { AuditLogsService } from '../audit-logs.service';

@Component({
    selector: 'app-audits-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {

    constructor(
        public vm: AuditLogsService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.vm.search();
    }

    public getRequestMethodClassNames(method: string): string {
        let classes = ['text-center'];
        switch (method.toLowerCase()) {
            case 'get':
                classes.push('text-primary');
                break;
            case 'post':
                classes.push('text-success');
                break;
            case 'put':
                classes.push('text-warning');
                break;
            case 'delete':
                classes.push('text-danger');
        }
        return classes.join(' ');

    }

}
