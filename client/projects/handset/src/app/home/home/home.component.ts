import { Component } from '@angular/core';

import { UiService } from '../../services/ui.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent {

    constructor(
        private ui: UiService
    ) { }

    public toggleDrawer(): void {
        this.ui.drawer.subscribe(drawer => {
            drawer.toggle();
        });
    }

}
