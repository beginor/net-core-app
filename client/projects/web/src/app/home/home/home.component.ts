import { Component, OnInit } from '@angular/core';
import { HomeService } from './home.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

    constructor(public vm: HomeService) { }

    public ngOnInit(): void { }

    public onClick(): void {
        this.vm.updateMessage();
    }

}
