import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class UiService {

    public alerts: Alert[] = [];

    constructor() { }

    public showAlert(alert: Alert): void {
        this.alerts.push(alert);
    }

    public closeAlert(alert: Alert): void {
        this.alerts.splice(this.alerts.indexOf(alert), 1);
    }

    public clearAlerts(): void {
        this.alerts = Array.from([]);
    }
}

export interface Alert {
    type: 'success' | 'info' | 'warning' | 'danger' | 'primary' | 'secondary'
        | 'light' | 'dark';
    message: string;
}
