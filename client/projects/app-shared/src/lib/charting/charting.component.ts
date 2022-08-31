import {
    Component, AfterViewInit, Input, ElementRef, ViewChild
} from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-charting', // eslint-disable-line @angular-eslint/component-selector, max-len
    templateUrl: './charting.component.html',
    styleUrls: ['./charting.component.css']
})
export class ChartingComponent implements AfterViewInit {

    @Input()
    public dataUrl!: string;
    @Input()
    public config!: string;

    @ViewChild('charting')
    public elRef!: ElementRef<HTMLDivElement>

    constructor(private http: HttpClient) { }

    public ngAfterViewInit(): void {
        // this.http.get(this.dataUrl).subscribe()
    }

}
