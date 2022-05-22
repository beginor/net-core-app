import { Component, OnInit } from '@angular/core';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

import { AccountService } from 'app-shared';

import { CategoryNode, CategoryService  } from '../../../common';

@Component({
    selector: 'app-data-categories-tree',
    templateUrl: './tree.component.html',
    styleUrls: ['./tree.component.css']
})
export class TreeComponent implements OnInit {

    constructor(
        public account: AccountService,
        public vm: CategoryService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.loadData();
    }

    public async loadData(): Promise<void> {
        await this.vm.getAll();
    }

    public onNewCategoryCancel(popover?: NgbPopover): void {
        if (!!popover) {
            popover.close();
        }
    }

    public onNewCategoryAdded(node: CategoryNode, pop?: NgbPopover): void {
        if (!!pop) {
            pop.close();
        }
        this.vm.nodes.getValue().push(node);
    }

}
