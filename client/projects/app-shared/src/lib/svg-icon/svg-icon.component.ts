import { Component, ElementRef, AfterViewInit, Input } from '@angular/core';

import { SvgIconService } from './svg-icon.service';

@Component({
    // tslint:disable-next-line: component-selector
    selector: 'svg-icon',
    template: `<svg width="1em" height="1em" viewBox="0 0 16 16" class="bi bi-app" fill="currentColor" xmlns="http://www.w3.org/2000/svg"><path fill-rule="evenodd" d="M11 2H5a3 3 0 0 0-3 3v6a3 3 0 0 0 3 3h6a3 3 0 0 0 3-3V5a3 3 0 0 0-3-3zM5 1a4 4 0 0 0-4 4v6a4 4 0 0 0 4 4h6a4 4 0 0 0 4-4V5a4 4 0 0 0-4-4H5z"/></svg>`,
    styles: [`:host { vertical-align: text-bottom; }`]
})
export class SvgIconComponent implements AfterViewInit {

    private viewInited = false;
    private iconPath = '';
    public get path(): string { return this.iconPath; }
    @Input() public set path(val: string) {
        if (val !== this.iconPath) {
            this.iconPath = val;
            if (!!this.iconPath && this.viewInited) {
                this.updateIcon();
            }
        }
    }
    @Input() public size = '1rem';
    @Input() public iconClass: string | undefined;

    constructor(
        private el: ElementRef<HTMLElement>,
        private svg: SvgIconService
    ) { }

    public async ngAfterViewInit(): Promise<void> {
        this.viewInited = true;
        const svg = this.el.nativeElement.firstChild as SVGElement;
        this.setIconProps(svg);
        if (!!this.iconPath) {
            await this.updateIcon();
        }
    }

    private async updateIcon(): Promise<void> {
        let svg = this.el.nativeElement.firstChild as SVGElement;
        const xml = await this.svg.loadSvgFile(this.path);
        svg.remove();
        this.el.nativeElement.innerHTML = xml;
        svg = this.el.nativeElement.firstChild as SVGElement;
        this.setIconProps(svg);
    }

    private setIconProps(svg: SVGElement): void {
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
