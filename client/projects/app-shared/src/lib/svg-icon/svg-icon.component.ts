import { Component, ElementRef, AfterViewInit, Input } from '@angular/core';

import { SvgIconService } from './svg-icon.service';

@Component({
    // tslint:disable-next-line: component-selector
    selector: 'svg-icon',
    template: ``,
    styles: [`
        /* :host { display: inline-block; } */
    `]
})
export class SvgIconComponent implements AfterViewInit {

    @Input() public path!: string;
    @Input() public size: string | undefined;
    @Input() public iconClass: string | undefined;

    constructor(
        private el: ElementRef<HTMLElement>,
        private svg: SvgIconService
    ) { }

    public async ngAfterViewInit(): Promise<void> {
        const xml = await this.svg.loadSvgFile(this.path);
        this.el.nativeElement.innerHTML = xml;
        const svg = this.el.nativeElement.firstChild as SVGElement;
        if (!!this.size) {
            svg.setAttribute('width', this.size);
            svg.setAttribute('height', this.size);
        }
        if (!!this.iconClass) {
            this.iconClass.split(' ').forEach(clx => {
                if (!svg.classList.contains(clx)) {
                    svg.classList.add(clx);
                }
            });
        }
        svg.setAttribute('fill', 'currentColor');
    }
}
