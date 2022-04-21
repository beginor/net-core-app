import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CategoryNode, CategoryService } from '../services/categories.service';

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
    
    public expanded = true;

    constructor(private vm: CategoryService) {
    }
    
    public hasChildren(): boolean {
        return !!this.node.children && this.node.children.length > 0;
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
        this.vm.currentNode.next(node);
    }
    
    public isSelected(node: CategoryNode): boolean {
        const currNode = this.vm.currentNode.getValue();
        const result = !!currNode && currNode.id === node.id;
        return result;
    }

}
