import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { ConfirmComponent } from '../confirm/confirm.component';

@Injectable({
    providedIn: 'root'
})
export class UiService {

    public alerts: Alert[] = [];

    constructor(
        private modal: NgbModal
    ) { }

    public showAlert(alert: Alert): void {
        this.alerts.push(alert);
    }

    public closeAlert(alert: Alert): void {
        this.alerts.splice(this.alerts.indexOf(alert), 1);
    }

    public clearAlerts(): void {
        this.alerts = Array.from([]);
    }

    public showConfirm(message: string): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            const ref = this.modal.open(
                ConfirmComponent,
                { size: 'sm', centered: true }
            );
            ref.componentInstance.message = message;
            ref.result.then(_ => resolve(true))
                .catch(_ => resolve(false));
        });
    }
}

export interface Alert {
    type: 'success' | 'info' | 'warning' | 'danger' | 'primary' | 'secondary'
        | 'light' | 'dark';
    message: string;
}
