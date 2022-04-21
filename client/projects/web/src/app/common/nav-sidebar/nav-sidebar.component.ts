import {
    trigger, animate, style, state, transition, AnimationEvent
} from '@angular/animations';
import { Component, EventEmitter, OnDestroy } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

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
export class NavSidebarComponent implements OnDestroy {

    public status = 'collapsed';
    public collapsed = true;
    public onToggle = new EventEmitter<boolean>(true);
    
    private destroyed = new Subject<void>();

    constructor(
        private bpo: BreakpointObserver,
        public navigation: NavigationService
    ) {
        this.bpo.observe(Breakpoints.XLarge)
            .pipe(takeUntil(this.destroyed))
            .subscribe(result => {
                if (result.matches) {
                    this.status = 'expanded';
                    this.collapsed = false;
                }
                else {
                    this.status = 'collapsed';
                    this.collapsed = true;
                }
            })
    }
    
    public ngOnDestroy(): void {
        this.destroyed.next();
        this.destroyed.complete();
    }

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
