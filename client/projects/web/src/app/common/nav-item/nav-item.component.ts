import { Component, OnChanges, Input, HostBinding } from '@angular/core';
import { Location } from '@angular/common';

import { NavigationNode, NavigationService } from '../services/navigation.service';

@Component({
    selector: 'app-nav-item',
    templateUrl: './nav-item.component.html',
    styleUrls: ['./nav-item.component.scss']
})
export class NavItemComponent implements OnChanges {

    @Input() public level = 1;
    @Input() public node!: NavigationNode;
    @Input() public sidebarCollapsed = true;
    @HostBinding('class.active') public get active(): boolean {
        return this.isActive();
    }

    public classes: { [index: string]: boolean } = { };
    public iconClasses = 'nav-icon';
    public expanded = false;

    constructor(
        public navigation: NavigationService,
        private location: Location
    ) { }

    public ngOnChanges(): void {
        this.setClasses();
    }

    private isActive(): boolean {
        const path = this.location.path(false);
        return path.startsWith(this.node.url as string);
    }

    private setClasses(): void {
        this.classes = {
            ['level-' + this.level]: true
        };
    }

}
