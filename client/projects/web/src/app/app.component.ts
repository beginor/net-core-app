import {
    trigger, animate, style, state, transition
} from '@angular/animations';
import { Component, ViewChild, TemplateRef } from '@angular/core';

import { AccountService } from 'app-shared';
import { UiService } from './common/services/ui.service';
import {
    NavSidebarComponent
} from './common/nav-sidebar/nav-sidebar.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    animations: [
        trigger('status', [
            state('expanded', style({ width: '220px' })),
            state('collapsed', style({ width: '48px' })),
            transition('expanded => collapsed', animate('.3s')),
            transition('collapsed => expanded', animate('.3s'))
        ])
    ]
})
export class AppComponent {

    public get sidebarStatus(): string {
        if (!!this.sidebar) {
            return this.sidebar.status;
        }
    }

    @ViewChild(NavSidebarComponent, { static: true })
    public sidebar: NavSidebarComponent;

    constructor(
        account: AccountService,
        public ui: UiService
    ) {
        account.getInfo().catch(ex => {
            console.error('get account info with error: ', ex);
        });
    }

    public onSidebarToggle(collapsed: boolean): void {
        console.log(collapsed);
    }

}
