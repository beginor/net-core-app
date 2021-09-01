import { Component } from '@angular/core';

import { NavigationService } from '../services/navigation.service';

@Component({
    selector: 'app-nav-card',
    template: `
      <div class="row g-0" *ngIf="(nav.sidebarNodes | async) as nodes">
        <div class="col-md-6 col-lg-3" *ngFor="let node of nodes">
          <div class="card m-2">
            <div class="card-body">
              <h5 class="card-title">
                <a class="card-link" [routerLink]="node.url">
                  {{node.title}}
                </a>
              </h5>
            </div>
          </div>
        </div>
      </div>
    `,
    styles: []
})
export class NavCardComponent {

    constructor(
        public nav: NavigationService
    ) { }

}
