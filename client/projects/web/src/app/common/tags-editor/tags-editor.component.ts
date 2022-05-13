import { Component, Input, OnInit } from '@angular/core';

@Component({
    selector: 'app-tags-editor',
    templateUrl: './tags-editor.component.html',
    styleUrls: ['./tags-editor.component.scss']
})
export class TagsEditorComponent {

    @Input()
    public editable: boolean = true;
    @Input()
    public tags: string[] = [];

    public newTag = '';
    
    public addNewTag(): void {
        const tag = this.newTag.trim();
        if (!!tag && this.tags.indexOf(tag) < 0) {
            this.tags.push(tag);
            this.newTag = '';
        }
    }
    
    public delTag(tag: string): void {
        const idx = this.tags.indexOf(tag);
        this.tags.splice(idx, 1);
    }

}
