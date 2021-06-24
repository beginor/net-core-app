import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { ServerFolderService, ServerFolderContent } from '../services/server-folder.service';

@Component({
    selector: 'app-server-folder-browser',
    templateUrl: './server-folder-browser.component.html',
    styleUrls: ['./server-folder-browser.component.scss']
})
export class ServerFolderBrowserComponent implements OnInit {

    public title?: string;
    public params!: ServerFolderContent;

    public items: FolderItem[] = [];

    constructor(
        public modal: NgbActiveModal,
        private service: ServerFolderService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.params = await this.service.getFolderContent(this.params);
        this.popupFolderItems();
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
        this.params = await this.service.getFolderContent(this.params);
        this.popupFolderItems();
    }

    private popupFolderItems(): void {
        this.items = [];
        this.params.folders?.forEach(x => {
            this.items.push({ name: x, type: 'folder', ext: '' });
        });
        this.params.files?.forEach(x => {
            const idx = x.indexOf('.');
            this.items.push({
                name: x.substring(0, idx),
                type: 'file',
                ext: x.substring(idx + 1)
            });
        });
    }

}

export interface FolderItem {
    name: string;
    type: 'folder' | 'file';
    ext: string;
}
