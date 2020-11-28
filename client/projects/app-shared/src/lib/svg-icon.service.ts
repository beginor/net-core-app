import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class SvgIconService {

    private basePath = 'assets/icons';
    private iconMap = new Map<string, string>();

    constructor(private http: HttpClient) {
    }

    public setBasePath(basePath: string): void {
        this.basePath = basePath;
    }

    public async loadSvgFile(path: string): Promise<string> {
        if (this.iconMap.has(path)) {
            return this.iconMap.get(path) as string;
        }
        const url = `${this.basePath}/${path}.svg`;
        const svg = await this.http.get(
            url, { responseType: 'text' }
        ).toPromise();
        if (svg.startsWith('<svg')) {
            this.iconMap.set(path, svg);
            return svg;
        }
        return '';
    }

}
