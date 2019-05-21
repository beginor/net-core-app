import { trigger, transition, useAnimation } from '@angular/animations';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-about',
    templateUrl: './about.component.html',
    styleUrls: ['./about.component.scss']
})
export class AboutComponent implements OnInit {

    public animation = 'bounce';

    constructor() { }

    public ngOnInit(): void { }

}
