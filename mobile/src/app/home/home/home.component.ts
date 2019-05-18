import { Component, OnInit } from '@angular/core';

import { UiService } from '../../services/ui.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

    constructor(
        private ui: UiService
    ) { }

    ngOnInit(): void {
    }

    toggleDrawer(): void {
        this.ui.drawer.subscribe(drawer => {
            drawer.toggle();
        });
    }

}
