import { Component, Input, OnInit } from '@angular/core';

import { CategoryNode, CategoryService } from '../services/categories.service';

@Component({
    selector: 'app-category-editor',
    templateUrl: './category-editor.component.html',
    styleUrls: ['./category-editor.component.scss']
})
export class CategoryEditorComponent implements OnInit {

    @Input()
    public category: { id?: string, name?: string; } = {};
    @Input()
    public editable = true;
    
    public nodes: CategoryNode[] = [];

    constructor(
        public vm: CategoryService
    ) {
        const nodes = this.vm.nodes.getValue();
        this.nodes = this.vm.getFlattenNodes(nodes);
    }

    public async ngOnInit(): Promise<void> {
        if (!this.category) {
            return;
        }
        if (!this.category.id) {
            this.category.id = this.nodes[0].id;
            this.category.name = this.nodes[0].name;
        }
    }
    
    public getStyle(level: number): { [key: string]: string } {
        return { 'padding-left': `${level + 1}rem`};
    }
    
    public updateCategory(node: CategoryNode): void {
        this.category.id = node.id;
        this.category.name = node.name;
    }

}
