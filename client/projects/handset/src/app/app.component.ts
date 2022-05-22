import {
    Component, AfterViewInit, ViewChild
} from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';

import { UiService } from './services/ui.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements AfterViewInit {

    @ViewChild(MatDrawer, { static: true })
    public drawer!: MatDrawer;

    constructor(
        private ui: UiService
    ) {
    }

    public ngAfterViewInit(): void {
        this.ui.setDrawer(this.drawer);
    }

}
