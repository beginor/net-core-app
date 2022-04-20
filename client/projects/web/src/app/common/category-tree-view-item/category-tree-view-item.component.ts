import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CategoryNode } from '../services/categories.service';

@Component({
    selector: 'app-category-tree-view-item',
    templateUrl: './category-tree-view-item.component.html',
    styleUrls: ['./category-tree-view-item.component.scss']
})
export class CategoryTreeViewItemComponent {
    
    @Input()
    public node!: CategoryNode;
    @Input()
    public level = 1;
    
    @Output()
    public itemClick = new EventEmitter<CategoryNode>();
    
    public expanded = true;

    constructor() {
    }
    
    public toggleExpand(): void {
        this.expanded = !this.expanded;
    }
    
    public getIconPath(): string {
        if (this.node.children.length === 0) {
            return 'bi/dash-square';
        }
        return this.expanded ? 'bi/dash-square' : 'bi/plus-square';
    }
    
    public onItemClick(node: CategoryNode): void {
        this.itemClick.next(node);
    }

}
