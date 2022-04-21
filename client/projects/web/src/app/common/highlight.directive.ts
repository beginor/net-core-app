import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
    selector: '[appHighlight]'
})
export class HighlightDirective {
    
    @Input()
    public appHighlight = 'bg-light';

    constructor(private elRef: ElementRef<HTMLElement>) {
    }
    
    @HostListener('mouseenter')
    public onMouseEnter(): void {
        if (!this.appHighlight) {
            return;
        }
        const el = this.elRef.nativeElement;
        const tokens = this.appHighlight.split(' ');
        tokens.forEach(token => {
            if (!!token && !el.classList.contains(token)) {
                el.classList.add(token);
            }
        });
    }
    
    @HostListener('mouseleave')
    public onMouseLeave(): void {
        const el = this.elRef.nativeElement;
        const tokens = this.appHighlight.split(' ');
        tokens.forEach(token => {
            if (!!token && el.classList.contains(token)) {
                el.classList.remove(token);
            }
        });
    }

}
