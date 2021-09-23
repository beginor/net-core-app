import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import {
    AccountService, UserTokenModel, UserTokenSearchModel
} from 'app-shared';
import { UiService } from '../../common';
import { TokenService } from '../token.service';

@Component({
    selector: 'app-token-list',
    templateUrl: './token-list.component.html',
    styleUrls: ['./token-list.component.scss']
})
export class TokenListComponent implements OnInit {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
        private account: AccountService,
        private ui: UiService,
        public vm: TokenService
    ) { }

    public ngOnInit(): void {
        void this.vm.loadTokens();
    }

    public showDetail(id: string): void {
        void this.router.navigate(
            ['./', id],
            { relativeTo: this.route, skipLocationChange: true }
        );
    }

    public deleteToken(id: string): void {
        //
    }

    public isExpires(date?: string): boolean {
        if (!date) {
            return false;
        }
        const d = new Date(date);
        return d < new Date();
    }

}
