import { Component, ElementRef, AfterViewInit, Input } from '@angular/core';

import { SvgIconService } from './svg-icon.service';

@Component({
    selector: 'svg-icon', // eslint-disable-line @angular-eslint/component-selector, max-len
    template: '<svg width="1em" height="1em" viewBox="0 0 16 16" class="bi bi-app" fill="currentColor" xmlns="http://www.w3.org/2000/svg"><path fill-rule="evenodd" d="M11 2H5a3 3 0 0 0-3 3v6a3 3 0 0 0 3 3h6a3 3 0 0 0 3-3V5a3 3 0 0 0-3-3zM5 1a4 4 0 0 0-4 4v6a4 4 0 0 0 4 4h6a4 4 0 0 0 4-4V5a4 4 0 0 0-4-4H5z"/></svg>', // eslint-disable-line max-len
    styles: []
})
export class SvgIconComponent implements AfterViewInit {

    private viewInited = false;
    private iconPath = '';
    private svgIconClass?: string;

    public get path(): string | undefined { return this.iconPath; }
    @Input() public set path(val: string | undefined) {
        if (!val) {
            return;
        }
        if (val !== this.iconPath) {
            this.iconPath = val;
            if (!!this.iconPath && this.viewInited) {
                void this.updateIcon();
            }
        }
    }
    @Input() public size = '1rem';

    public get iconClass(): string | undefined { return this.svgIconClass }
    @Input() public set iconClass(val: string | undefined) {
        if (val === this.svgIconClass) {
            return;
        }
        const oldClasses = this.svgIconClass;
        this.svgIconClass = val;
        if (!this.viewInited) {
            return;
        }
        const svg = this.el.nativeElement.firstChild as SVGElement;
        if (!svg) {
            this.removeClasses(svg, oldClasses);
            this.addClasses(svg, val);
        }
    }

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
        if (!this.path) {
            return;
        }
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
        this.addClasses(svg, this.svgIconClass);
        svg.setAttribute('fill', 'currentColor');
    }

    private removeClasses(el: SVGElement, classes?: string): void {
        if (!classes) {
            return;
        }
        classes.split(' ').forEach(c => {
            if (el.classList.contains(c)) {
                el.classList.remove(c);
            }
        });
    }

    private addClasses(el: SVGElement, classes?: string): void {
        if (!classes) {
            return;
        }
        classes.split(' ').forEach(c => {
            if (!el.classList.contains(c)) {
                el.classList.add(c);
            }
        });
    }
}
