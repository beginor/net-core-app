import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-confirm',
    templateUrl: './confirm.component.html',
    styleUrls: ['./confirm.component.scss']
})
export class ConfirmComponent implements OnInit {

    public title = '提示：';
    public message = '确认么？';

    constructor(
        public modal: NgbActiveModal
    ) { }

    public ngOnInit(): void {
    }

}
