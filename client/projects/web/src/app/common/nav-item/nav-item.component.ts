import { Component, OnChanges, Input } from '@angular/core';

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
    public nodeChildren: NavigationNode[];

    public ngOnChanges(): void {
        this.nodeChildren = (this.node && this.node.children)
            ? this.node.children.filter(n => !n.hidden) : [];
        this.setClasses();
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

}
