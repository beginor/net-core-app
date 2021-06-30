import {
    trigger, animate, style, state, transition, AnimationEvent
} from '@angular/animations';
import { Component, EventEmitter } from '@angular/core';

import { NavigationService } from '../services/navigation.service';

@Component({
    selector: 'app-nav-sidebar',
    templateUrl: './nav-sidebar.component.html',
    styleUrls: ['./nav-sidebar.component.scss'],
    animations: [
        trigger('status', [
            state('expanded', style({ width: '220px' })),
            state('collapsed', style({ width: '53px' })),
            transition('expanded => collapsed', animate('.3s')),
            transition('collapsed => expanded', animate('.3s'))
        ])
    ]
})
export class NavSidebarComponent {

    public status = 'expanded';
    public collapsed = false;
    public onToggle = new EventEmitter<boolean>(true);

    constructor(
        public navigation: NavigationService
    ) { }

    public toggle(): void {
        if (this.status === 'collapsed') {
            this.status = 'expanded';
        }
        else {
            this.status = 'collapsed';
        }
        // this.onToggle.next(this.status === 'collapsed');
    }

    public onAnimationStart(e: AnimationEvent): void {
        if (e.toState === 'collapsed') {
            this.collapsed = true;
        }
    }

    public onAnimationDone(e: AnimationEvent): void {
        if (e.toState === 'expanded') {
            this.collapsed = false;
        }
    }

}
