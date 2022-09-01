import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom } from 'rxjs';
import { EChartsOption } from 'echarts';

@Injectable({
    providedIn: 'root'
})
export class EchartService {

    constructor(
        private http: HttpClient,
        @Inject('apiRoot') private apiRoot: string,
    ) { }

    public async loadConfig(config: string): Promise<EchartProps> {
        const text = await lastValueFrom(
            this.http.get(config, { responseType: 'text' })
        );
        // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment
        const props = JSON.parse(text, (key, val) => {
            if (typeof val === 'string') {
                if (val.startsWith('(') && val.endsWith('}')) {
                    // eslint-disable-next-line max-len
                    // eslint-disable-next-line @typescript-eslint/no-unsafe-return
                    return (0, eval)(`(${val})`);
                }
                if (val.startsWith('--')) {
                    return getComputedStyle(
                        self.document.documentElement
                    ).getPropertyValue(val);
                }
            }
            if (Array.isArray(val)) {
                if (val[0] === '_lambda' && val[val.length - 1] === '}') {
                    const lambda = val.slice(1).join('\n');
                    // eslint-disable-next-line max-len
                    // eslint-disable-next-line @typescript-eslint/no-unsafe-return
                    return (0, eval)(`(${lambda})`);
                }
            }
            // eslint-disable-next-line @typescript-eslint/no-unsafe-return
            return val;
        });
        return props as EchartProps;
    }

    public async loadData(url: string): Promise<{ data: any[]; }> {
        const dataUrl = url.startsWith(this.apiRoot) ? url : this.apiRoot + url;
        return await lastValueFrom(
            this.http.get<{ data: any[] }>(dataUrl)
        );
    }

}

export interface EchartProps {
    style?: CSSStyleDeclaration;
    data: {
        url: string;
    };
    echarts: EChartsOption;
    beforeSetChartOptions?: (options: EChartsOption) => void;
}
