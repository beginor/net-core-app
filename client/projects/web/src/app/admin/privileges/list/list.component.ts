import { Component, OnInit } from '@angular/core';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';


import { AccountService } from 'app-shared';
import { AppPrivilegeService } from '../privileges.service';
import { DetailComponent } from '../detail/detail.component';

@Component({
    selector: 'app-privilege-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css'],
})
export class ListComponent implements OnInit {

    constructor(
        private offcanvas: NgbOffcanvas,
        public account: AccountService,
        public vm: AppPrivilegeService
    ) { }

    public ngOnInit(): void {
        this.loadData();
    }

    public showDetail(id: string, editable: boolean): void {
        const ref = this.offcanvas.open(
            DetailComponent,
            { position: 'end', panelClass: 'offcanvas-vw-40' }
        );
        const detail = ref.componentInstance as DetailComponent;
        detail.editable = editable;
        detail.id = id;
        void ref.result.then(() => {
            void this.vm.search();
        });
    }

    public async loadData(): Promise<void> {
        await this.vm.getModules();
        await this.vm.search();
    }

    public async delete(id: string): Promise<void> {
        const deleted = await this.vm.delete(id);
        if (deleted) {
            void this.vm.search();
        }
    }

}
