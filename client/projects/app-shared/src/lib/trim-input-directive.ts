import { Directive, HostListener, OnInit, Optional } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
    selector: 'input[trimInput],textarea[trimInput]', // eslint-disable-line @angular-eslint/directive-selector, max-len
})
export class TrimInputDirective implements OnInit {

    constructor(@Optional() private ngControl: NgControl) {
    }

    public ngOnInit(): void {
        if (!this.ngControl) {
            console.warn('TrimInputDirective required ngModel, formControl or formControlName.'); // eslint-disable-line max-len, no-console
            return;
        }
    }

    @HostListener('blur', ['$event.target', '$event.target.value',])
    public onBlur(el: HTMLInputElement, value: string): void {
        if ('function' === typeof value.trim && value.trim() !== value) {
            el.value = value.trim();
            let e = new Event('input', { bubbles: false, cancelable: false });
            el.dispatchEvent(e);

            e = new Event('blur', { bubbles: false, cancelable: false });
            el.dispatchEvent(e);
        }
    }

}
