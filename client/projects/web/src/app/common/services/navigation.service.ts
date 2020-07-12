import { Injectable, Inject, ErrorHandler } from '@angular/core';
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
    public topbarNodes = new BehaviorSubject<NavigationNode[] | undefined>([]);
    public sidebarNodes = new BehaviorSubject<NavigationNode[] | undefined>([]);
    public initialized = false;
    public showSidebar = false;

    private currentUrl: string;

    constructor(
        private http: HttpClient,
        private title: Title,
        private location: Location,
        private account: AccountService,
        @Inject('apiRoot') private apiRoot: string,
        private errorHandler: ErrorHandler
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
        return this.location.path().includes(node.url as string);
    }

    public isRouterLink(node: NavigationNode): boolean {
        if (!node.url) {
            return false;
        }
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
        if (!!rootNode.children) {
            this.sidebarNodes.next(
                this.findSidebarNavigationNodes(rootNode.children)
            );
        }
    }

    private updateSidebarNodes(): void {
        const rootNode = this.root.getValue();
        if (!rootNode) {
            return;
        }
        const path = this.location.path();
        if (rootNode.children) {
            for (const node of rootNode.children) {
                if (!node.url) {
                    continue;
                }
                if (path.startsWith(node.url)) {
                    this.sidebarNodes.next(node.children);
                    this.showSidebar = !!node.children && node.children.length > 0;
                    break;
                }
            }
        }
        this.currentUrl = path;
    }

    private findSidebarNavigationNodes(
        topNodes: NavigationNode[]
    ): NavigationNode[] {
        const path = this.location.path(false);
        for (const child of topNodes) {
            if (!!child.url && path.startsWith(child.url)) {
                if (!!child.children) {
                    return child.children;
                }
            }
        }
        return [];
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
                this.updateTitle(node.title as string);
                if (!this.initialized) {
                    this.initialized = true;
                }
            })
            .catch(ex => {
                this.errorHandler.handleError(ex);
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
