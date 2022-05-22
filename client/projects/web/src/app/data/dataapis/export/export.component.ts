import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UserTokenModel } from 'app-shared';

import { DataApiDocModel, DataApiService } from '../dataapis.service';

@Component({
    selector: 'app-dataapis-export',
    templateUrl: './export.component.html',
    styleUrls: ['./export.component.css']
})
export class ExportComponent implements OnInit {

    public token: UserTokenModel = { id: '', name: '', value: '', urls: [] };

    public model: DataApiDocModel = {
        title: '',
        description: '',
        format: 'json',
        apis: [],
        token: '',
        referer: undefined
    };

    constructor(
        public activeModal: NgbActiveModal,
        public vm: DataApiService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.vm.loadTokens();
        const tokens = this.vm.tokens.getValue();
        if (tokens.length > 0) {
            this.token = tokens[0];
        }
    }

    public async exportApiDoc(): Promise<void> {
        this.model.token = this.token.value;
        if (!this.token.urls || this.token.urls.length < 0) {
            this.model.referer = undefined;
        }
        const result = await this.vm.exportApiDoc(this.model);
        if (result.length > 0) {
            const ext = this.model.format === 'json' ? 'json' : 'md';
            const type = this.model.format === 'json' ? 'application/json' : 'text/markdown'; // eslint-disable-line max-len
            const fileName = `${this.model.title}.${ext}`;
            const props = { type };
            let file: File | Blob;
            try {
                file = new File([result], fileName, props);
            }
            catch (ex: any) {
                file = new Blob([result], props)
            }
            const link = document.createElement('a');
            link.download = fileName;
            link.href = URL.createObjectURL(file);
            link.click();
            this.activeModal.close('ok');
        }
        this.activeModal.dismiss('');
    }

}
