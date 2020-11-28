import { Directive, ElementRef, Input, OnInit } from '@angular/core';

import { SvgIconService } from './svg-icon.service';

@Directive({
    // tslint:disable-next-line: directive-selector
    selector: '[svgIcon]'
})
export class SvgIconDirective implements OnInit {

    @Input() public path!: string;
    @Input() public size: string | undefined;
    @Input() public iconClass: string | undefined;

    constructor(
        private el: ElementRef<HTMLElement>,
        private svg: SvgIconService
    ) { }

    public async ngOnInit(): Promise<void> {
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
