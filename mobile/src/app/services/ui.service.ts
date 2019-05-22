import { Injectable } from '@angular/core';
import { MatDrawer } from '@angular/material';

import { AsyncSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UiService {

    public drawer = new AsyncSubject<MatDrawer>();

    constructor() { }

    public setDrawer(drawer: MatDrawer): void {
        this.drawer.next(drawer);
        this.drawer.complete();
    }

}
