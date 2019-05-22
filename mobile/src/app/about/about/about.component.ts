import { Component, OnInit } from '@angular/core';

import { UiService } from '../../services/ui.service';

@Component({
    selector: 'app-about',
    templateUrl: './about.component.html',
    styleUrls: ['./about.component.scss']
})
export class AboutComponent implements OnInit {

    constructor(
        private ui: UiService
    ) { }

    public ngOnInit(): void {
    }

    public toggleDrawer(): void {
        this.ui.drawer.subscribe(drawer => {
            drawer.toggle();
        });
    }

}
