import { Component, OnChanges, Input } from '@angular/core';
import { Location } from '@angular/common';

import { NavigationNode } from '../services/navigation.service';

@Component({
    selector: 'app-nav-item',
    templateUrl: './nav-item.component.html',
    styleUrls: ['./nav-item.component.scss']
})
export class NavItemComponent implements OnChanges {

    @Input() public level = 1;
    @Input() public node: NavigationNode;
    @Input() public collapsed = true;

    public classes: { [index: string]: boolean };
    public iconClasses = 'nav-icon';
    public expanded = false;

    constructor(
        private location: Location
    ) { }

    public ngOnChanges(): void {
        this.setClasses();
        const path = this.location.path(false);
        if (path.startsWith(this.node.url)) {
            this.expanded = true;
        }
    }

    private setClasses(): void {
        this.classes = {
            ['level-' + this.level]: true
        };
        let iconClasses = 'nav-icon ml-2';
        if (!this.node.icon) {
            iconClasses += ' fas fa-info';
        }
        else {
            iconClasses += (' ' + this.node.icon);
        }
        this.iconClasses = iconClasses;
    }

    public toggle($e: Event): void {
        $e.preventDefault();
        $e.stopPropagation();
        this.expanded = !this.expanded;
    }

}
