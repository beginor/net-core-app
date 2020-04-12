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

    public root = new BehaviorSubject<NavigationNode>(
        { title: '', children: [] }
    );
    public topbarNodes = new BehaviorSubject<NavigationNode[]>([]);
    public sidebarNodes = new BehaviorSubject<NavigationNode[]>([]);
    public initialized: boolean;

    private currentUrl: string;

    constructor(
        private http: HttpClient,
        private title: Title,
        private location: Location,
        private account: AccountService,
        @Inject('apiRoot') private apiRoot: string,
    ) {
        this.currentUrl = location.path();
        this.location.onUrlChange(() => {
            this.updateSidebarNodes();
        });
        account.info.subscribe(() => {
            this.loadAccountMenu();
        });
    }

    public isActiveNode(node: NavigationNode): boolean {
        return this.location.path().includes(node.url);
    }

    public isRouterLink(node: NavigationNode): boolean {
        if (node.url.startsWith('https://') || node.url.startsWith('http://')) {
            return false;
        }
        if (!!node.target) {
            return false;
        }
        return true;
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

    private loadAccountMenu(): void {
        const menuUrl = `${this.apiRoot}/account/menu`;
        this.http.get<NavigationNode>(menuUrl)
            .toPromise()
            .then(node => {
                this.setupNavigationNodes(node);
                this.updateTitle(node.title);
                if (!this.initialized) {
                    this.initialized = true;
                }
            })
            .catch(ex => {
                console.error(ex);
            });
    }

}

export interface NavigationNode {
    id?: string;
    title?: string;
    url?: string;
    tooltip?: string;
    hidden?: boolean;
    icon?: string;
    target?: string;
    children?: NavigationNode[];
}
