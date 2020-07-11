import { ValidatorFn, AbstractControl } from '@angular/forms';

/**
 * Confirm to validator function;
 * @param targetField targetField
 */
export function confirmTo(
    targetField: string
): ValidatorFn {
    return (control: AbstractControl) => {
        const form = control.root;
        const target = form.get(targetField);
        if (!target) {
            return null;
        }
        if (target.value !== control.value) {
            return { confirmTo: true };
        }
        return null;
    };
}
