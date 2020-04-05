import { Injectable, Inject } from '@angular/core';
import { Location } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Title } from '@angular/platform-browser';

import { BehaviorSubject } from 'rxjs';
import { AccountService } from 'app-shared';

@Injectable({
    providedIn: 'root'
})
export class NavigationService {

    public root = new BehaviorSubject<NavigationNode>({ title: '' });
    public topbarNodes = new BehaviorSubject<NavigationNode[]>([]);
    public sidebarNodes = new BehaviorSubject<NavigationNode[]>([]);

    private currentUrl: string;

    constructor(
        private http: HttpClient,
        private title: Title,
        private location: Location,
        private account: AccountService,
        @Inject('apiRoot') private apiRoot: string,
    ) {
        // this.loadAccountMenu();
        this.currentUrl = location.path();
        this.location.onUrlChange(() => {
            this.updateSidebarNodes();
        });
        account.info.subscribe(() => {
            this.loadAccountMenu();
        });
    }

    private loadAccountMenu(): void {
        const menuUrl = `${this.apiRoot}/account/menu`;
        this.http.get<NavigationNode>(menuUrl)
            .toPromise()
            .then(node => {
                this.setupNavigationNodes(node);
                this.updateTitle(node.title);
            })
            .catch(ex => {
                console.error(ex);
            });
    }

    public isActiveNode(node: NavigationNode): boolean {
        return this.location.path().includes(node.url);
    }

    private setupNavigationNodes(rootNode: NavigationNode): void {
        this.root.next(rootNode);
        this.topbarNodes.next(rootNode.children);
        this.sidebarNodes.next(
            this.findSidebarNavigationNodes(rootNode.children)
        );
    }

    private updateSidebarNodes(): void {
        const rootNode = this.root.getValue();
        if (!rootNode) {
            return;
        }
        const path = this.location.path();
        for (const node of rootNode.children) {
            if (path.startsWith(node.url)) {
                this.sidebarNodes.next(node.children);
                break;
            }
        }
        this.currentUrl = path;
    }

    private findSidebarNavigationNodes(
        topNodes: NavigationNode[]
    ): NavigationNode[] {
        const path = this.location.path(false);
        for (const child of topNodes) {
            if (path.startsWith(child.url)) {
                return child.children;
            }
        }
    }

    private updateTitle(title: string): void {
        this.title.setTitle(title);
    }

}

export interface NavigationNode {
    title: string;
    url?: string;
    tooltip?: string;
    hidden?: boolean;
    icon?: string;
    children?: NavigationNode[];
}
