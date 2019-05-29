import { Injectable } from '@angular/core';
import { Subject, BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root'})
export class HomeService {

    public message = new BehaviorSubject<string>('Hello, angular !');
    public info = new Subject<string>();

    private count = 0;

    public updateMessage(): void {
        this.count++;
        const msg = `You have clicked ${this.count} times !`;
        this.message.next(msg);

        if (this.count % 5 === 0) {
            this.info.next('Well done!');
        }
    }

}
