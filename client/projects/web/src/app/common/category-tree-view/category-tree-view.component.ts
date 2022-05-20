import {
    Component, EventEmitter, Input, OnDestroy, OnInit, Output
} from '@angular/core';

import { Subscription } from 'rxjs';

import { CategoryNode, CategoryService } from '../services/categories.service';

@Component({
    selector: 'app-category-tree-view',
    templateUrl: './category-tree-view.component.html',
    styleUrls: ['./category-tree-view.component.scss']
})
export class CategoryTreeViewComponent implements OnInit, OnDestroy {
    @Input()
    public resourceType?: string
    @Output()
    public itemClick = new EventEmitter<CategoryNode>();
    
    public currentNode?: CategoryNode;
    
    private currentNode$?: Subscription;

    constructor(
        public vm: CategoryService
    ) {
    }

    public ngOnInit(): void {
        this.currentNode$ = this.vm.currentNode.subscribe(node => {
            this.currentNode = node;
            this.itemClick.next(node!);
        });
        if (this.vm.nodes.getValue().length === 0) {
            void this.loadData();
        }
    }
    
    public ngOnDestroy(): void {
        if (!!this.currentNode$) {
            this.currentNode$.unsubscribe();
        }
    }

    public refresh(): void {
        void this.loadData();
    }
    
    public onItemClick(node: CategoryNode): void {
        this.currentNode = node;
        this.itemClick.next(node);
    }
    
    public async loadData(): Promise<void> {
        await this.vm.getAll(this.resourceType);
    }

}
