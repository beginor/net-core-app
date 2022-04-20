import { Component, OnInit, Input } from '@angular/core';

import { CategoryNode, CategoryService  } from '../../../common';

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

    public status: Status = 'view';

    constructor(
        public vm: CategoryService
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
        this.status = 'view';
        if (newNode.parentId == this.node.id) {
            this.node.children.push(newNode);
        }
    }

    public onEditCancel(): void {
        this.status = 'view';
    }

    public async deleteNode(): Promise<void> {
        const deleted = await this.vm.delete(this.node.id as string);
        if (deleted) {
            const parent = this.vm.findParent(this.node);
            if (!!parent) {
                const index = parent.children.indexOf(this.node);
                parent.children.splice(index, 1);
            }
            else {
                const roots = this.vm.nodes.getValue();
                const index = roots.indexOf(this.node);
                roots.splice(index, 1);
            }
        }
    }

    public startEdit(): void {
        this.status = 'edit';
    }

    public toggleAdd(): void {
        this.status = this.status === 'add' ? 'view' : 'add';
    }

}

export type Status = 'view' | 'edit' | 'add';
