import { Component, OnInit, EventEmitter } from '@angular/core';

@Component({
    selector: 'app-nav-sidebar',
    templateUrl: './nav-sidebar.component.html',
    styleUrls: ['./nav-sidebar.component.scss']
})
export class NavSidebarComponent implements OnInit {

    public status = 'expanded';
    public get collapsed(): boolean {
        return this.status === 'collapsed';
    }
    public onToggle = new EventEmitter<boolean>(true);

    constructor() { }

    public ngOnInit(): void {
    }

    public toggle(): void {
        if (this.status === 'collapsed') {
            this.status = 'expanded';
        }
        else {
            this.status = 'collapsed';
        }
        this.onToggle.next(this.status === 'collapsed');
    }

    public getToggleIconClass(): string {
        let cls = 'ml-2 fa fa-fw fa-angle-double-';
        cls += (this.collapsed ? 'right' : 'left');
        return cls;
    }

}
