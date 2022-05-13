import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { CategoryNode, CategoryService } from '../services/categories.service';

@Component({
    selector: 'app-category-editor',
    templateUrl: './category-editor.component.html',
    styleUrls: ['./category-editor.component.scss']
})
export class CategoryEditorComponent implements OnInit, OnDestroy {
    
    private node$: Subscription;
    
    @Input()
    public category: { id?: string, name?: string; } = {};
    @Input()
    public editable = true;
    
    public nodes: CategoryNode[] = [];

    constructor(
        public vm: CategoryService
    ) {
        this.node$ = this.vm.nodes.subscribe(nodes => {
            this.nodes = this.vm.getFlattenNodes(nodes);
        });
    }

    public async ngOnInit(): Promise<void> {
        await this.vm.getAll()
        if (!this.category) {
            return;
        }
        if (!this.category.id) {
            this.category.id = this.nodes[0].id;
            this.category.name = this.nodes[0].name;
        }
    }
    
    public ngOnDestroy(): void {
        this.node$.unsubscribe();
    }
    
    public getStyle(level: number): { [key: string]: string } {
        return { 'padding-left': `${level + 1}rem`};
    }
    
    public updateCategory(node: CategoryNode): void {
        this.category.id = node.id;
        this.category.name = node.name;
    }

}
