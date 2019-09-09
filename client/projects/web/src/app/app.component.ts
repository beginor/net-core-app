import { Component } from '@angular/core';
import { UiService } from './common/services/ui.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {

    public collapsed = true;

    constructor(
        public ui: UiService
    ) {
    }

}
