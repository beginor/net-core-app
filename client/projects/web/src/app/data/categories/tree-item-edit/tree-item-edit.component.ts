import {
    Component, Input, Output, EventEmitter
} from '@angular/core';

import {
    CategoryModel, CategoryNode, CategoryService
} from '../categories.service';

@Component({
    selector: 'app-data-categories-tree-item-edit',
    templateUrl: './tree-item-edit.component.html',
    styleUrls: ['./tree-item-edit.component.scss']
})
export class TreeItemEditComponent {

    @Input()
    public node: CategoryNode;
    @Input()
    public compact = false;

    @Output()
    public cancel = new EventEmitter<void>();

    @Output()
    public updated = new EventEmitter<CategoryNode>();

    constructor(
        private vm: CategoryService
    ) {
        this.node = vm.createEmptyNode();
    }

    public getFormClass(): string {
        return this.compact ? ''
            : 'p-2 bg-white border border-1 flex-grow-1 d-flex flex-row align-items-center';
    }

    public async saveOrUpdate(node: CategoryNode): Promise<void> {
        let model: CategoryModel | undefined;
        if (!node.id) {
            model = await this.vm.create(node as CategoryModel);
        }
        else {
            model = await this.vm.update(node.id, node as CategoryModel);
        }
        if (!!model) {
            this.node = this.vm.createEmptyNode();
            const newNode = model as CategoryNode;
            newNode.children = [];
            this.updated.next(newNode);
        }
    }

}
