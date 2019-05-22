import { Injectable } from '@angular/core';

import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class NavMenuService {

    public menuItems: BehaviorSubject<MenuItem[]>;

    constructor() {
        const items: MenuItem[] = [
            { icon: 'home', text: '首页', link: '/home'},
            { icon: 'info', text: '关于', link: '/about' }
        ];
        this.menuItems = new BehaviorSubject<MenuItem[]>(items);
    }
}

export interface MenuItem {
    icon: string;
    link: string;
    text: string;
}
