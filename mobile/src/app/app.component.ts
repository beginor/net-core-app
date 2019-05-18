import {
    Component, AfterViewInit, ViewChild
} from '@angular/core';
import { MatDrawer } from '@angular/material';

import { UiService } from './services/ui.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements AfterViewInit {

    @ViewChild(MatDrawer)
    drawer: MatDrawer;

    constructor(
        private ui: UiService
    ) {
    }

    ngAfterViewInit(): void {
        this.ui.setDrawer(this.drawer);
    }

}
