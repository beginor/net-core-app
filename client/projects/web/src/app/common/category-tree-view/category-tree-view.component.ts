import { Component, EventEmitter, OnInit, Output } from '@angular/core';

import { CategoryNode, CategoryService } from '../services/categories.service';

@Component({
    selector: 'app-category-tree-view',
    templateUrl: './category-tree-view.component.html',
    styleUrls: ['./category-tree-view.component.scss']
})
export class CategoryTreeViewComponent implements OnInit {
    
    @Output()
    public itemClick = new EventEmitter<CategoryNode>();
    
    public currentNode?: CategoryNode;

    constructor(
        public vm: CategoryService
    ) {
    }

    public ngOnInit(): void {
        if (this.vm.nodes.getValue().length === 0) {
            void this.vm.getAll();
        }
    }
    
    public refresh(): void {
        void this.vm.getAll();
    }
    
    public onItemClick(node: CategoryNode): void {
        this.currentNode = node;
        this.itemClick.next(node);
    }

}
