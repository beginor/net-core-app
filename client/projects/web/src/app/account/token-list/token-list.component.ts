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
    styleUrls: ['./token-list.component.css']
})
export class TokenListComponent implements OnInit {

    constructor(
        private router: Router,
        private route: ActivatedRoute,
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

    public async deleteToken(id: string): Promise<void> {
        const deleted = await this.vm.delete(id);
        if (deleted) {
            void this.vm.loadTokens();
        }
    }

    public isExpires(date?: string): boolean {
        if (!date) {
            return false;
        }
        const d = new Date(date);
        return d < new Date();
    }

    public resetSearch(): void {
        this.vm.model.keywords = '';
        void this.vm.loadTokens();
    }

}
