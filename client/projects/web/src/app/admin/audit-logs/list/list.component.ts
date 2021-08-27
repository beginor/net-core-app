import { Component, OnInit } from '@angular/core';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap';

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

    public getRequestMethodClasses(method: string): string {
        const classes = ['text-center'];
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

    public getDurationClasses(duration: number): string {
        const classes = ['text-end'];
        if (duration < 1000) {
            classes.push('text-success');
        }
        else if (duration < 2000) {
            classes.push('text-warning');
        }
        else {
            classes.push('text-danger');
        }
        return classes.join(' ');
    }

    public getResponseCodeClasses(code: number): string {
        const classes = ['text-end'];
        if (code < 300) {
            classes.push('text-success');
        }
        else if (code < 500) {
            classes.push('text-warning');
        }
        else {
            classes.push('text-danger');
        }
        return classes.join(' ');
    }

    public async onSelectDate(d: NgbDate): Promise<void> {
        // this.vm.searchDate = d;
        this.vm.searchModel.skip = 0;
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

    public onUserNameChanged(): void {
        this.vm.searchModel.skip = 0;
        void this.vm.search();
    }

    public resetUsername(): void {
        this.vm.searchModel.userName = '';
        void this.vm.search();
    }

}
