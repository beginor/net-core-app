import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { StorageService, StorageContent } from '../services/storage.service';

@Component({
    selector: 'app-storage-browser',
    templateUrl: './storage-browser.component.html',
    styleUrls: ['./storage-browser.component.css']
})
export class StorageBrowserComponent implements OnInit {

    public title?: string;
    public params!: StorageContent;

    public filteredItems: FolderItem[] = [];
    private allItems: FolderItem[] = [];
    public selectedItem?: FolderItem;
    public breadcrumbs: BreadCrumb[] = [];
    public searchFilter?: string;

    @ViewChild('searchEl')
    public searchEl?: ElementRef<HTMLInputElement>;

    constructor(
        public modal: NgbActiveModal,
        private service: StorageService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.loadData();
    }

    public async getFolderContentByPath(path: string): Promise<void> {
        this.params.path = path ?? '.';
        await this.loadData();
    }

    public async getFolderContent(item: FolderItem): Promise<void> {
        if (item.type === 'file') {
            return;
        }
        let path = this.params.path;
        if (!path) {
            path = item.name;
        }
        else {
            if (path.endsWith('.') || path.endsWith('/')) {
                path = path.substr(0, path.length - 1);
            }
            path = `${path}/${item.name}`;
        }
        this.params.path = path;
        await this.loadData();
    }

    public getFolderItemClass(item: FolderItem): string {
        if (item.type === 'folder') {
            return 'text-info';
        }
        if (item.type === 'file' && item.ext === 'svg') {
            return 'text-info';
        }
        return 'text-muted';
    }

    public getFolderItemIconPath(item: FolderItem): string {
        if (item.type === 'folder') {
            return 'bi/folder';
        }
        if (item.type === 'file' && item.ext === 'svg') {
            return `${this.params.path}/${item.name}`;
        }
        return 'bi/file-earmark';
    }

    public setSelectedItem(item: FolderItem): void {
        this.selectedItem = item;
    }

    public ok(): void {
        let path = this.params.path;
        if (path === '.') {
            path = '';
        }
        if (path !== '') {
            path += '/';
        }
        let file = this.selectedItem?.name;
        if (this.selectedItem?.type === 'file') {
            file += '.' + this.selectedItem.ext;
        }
        path += file;
        this.modal.close(path);
    }

    public search(e: KeyboardEvent): void {
        const input = e.target as HTMLInputElement;
        const filter = input.value;
        if (!!filter) {
            this.filteredItems = this.allItems.filter(
                x => x.name.indexOf(filter) > -1
            );
        }
        else {
            this.filteredItems = this.allItems;
        }
    }

    private async loadData(): Promise<void> {
        if (!!this.searchEl) {
            this.searchEl.nativeElement.value = '';
        }
        this.allItems = [];
        this.filteredItems = [];
        // this.breadcrumbs = [];
        this.params = await this.service.getFolderContent(this.params);
        this.buildFolderItems();
        this.buildBreadcrumbs();
    }

    private buildFolderItems(): void {
        this.allItems = [];
        this.params.folders?.sort().forEach(x => {
            this.allItems.push({ name: x, type: 'folder', ext: '' });
        });
        this.params.files?.sort().forEach(x => {
            const idx = x.lastIndexOf('.');
            this.allItems.push({
                name: x.substring(0, idx),
                type: 'file',
                ext: x.substring(idx + 1)
            });
        });
        this.filteredItems = this.allItems;
    }

    private buildBreadcrumbs(): void {
        const path = this.params.path;
        if (path !== '.') {
            const segments = path.split('/');
            this.breadcrumbs = segments.map((segment, idx) => {
                const breadcrumb: BreadCrumb = {
                    name: segment,
                    path: segments.slice(0, idx + 1).join('/')
                };
                return breadcrumb;
            });
        }
        else {
            this.breadcrumbs = [];
        }
    }

}

export interface FolderItem {
    name: string;
    type: 'folder' | 'file';
    ext: string;
}

export interface BreadCrumb {
    name: string;
    path: string;
}
