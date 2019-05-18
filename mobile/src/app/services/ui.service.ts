import { Injectable } from '@angular/core';
import { MatDrawer } from '@angular/material';

import { AsyncSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UiService {

    drawer = new AsyncSubject<MatDrawer>();

    constructor() { }

    setDrawer(drawer: MatDrawer): void {
        this.drawer.next(drawer);
        this.drawer.complete();
    }

}
