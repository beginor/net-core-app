import { Component, OnInit, Input } from '@angular/core';

import {
    CategoryModel, CategoryNode, CategoryService
} from '../categories.service';

@Component({
    selector: 'app-data-categories-tree-item',
    templateUrl: './tree-item.component.html',
    styleUrls: ['./tree-item.component.scss']
})
export class TreeItemComponent implements OnInit {

    @Input()
    public node!: CategoryNode;
    @Input()
    public expanded = true;
    @Input()
    public canEdit = false;
    @Input()
    public canDelete = false;
    @Input()
    public canAddChild = false;

    public showAddChildUI = false;

    constructor(
        private vm: CategoryService
    ) { }

    public ngOnInit(): void {
        this.expanded = (!!this.node.children && this.node.children.length > 0);
    }

    public getTreeNodeIcon(): string {
        return this.expanded ? 'bi/dash-square' : 'bi/plus-square';
    }

    public toggleExpand(): void {
        this.expanded = !this.expanded;
    }

    public onEditUpdated(newNode: CategoryNode): void {
        this.showAddChildUI = false;
        if (newNode.parentId == this.node.id) {
            this.node.children.push(newNode);
        }
    }

    public onEditCancel(): void {
        this.showAddChildUI = false;
    }

    public async deleteNode(id: string): Promise<void> {
        const deleted = await this.vm.delete(id);
        if (deleted) {
            await this.vm.getAll();
        }
    }

}
