import {
    Component, AfterViewInit, ElementRef, ViewChild, Input, OnDestroy, NgZone
} from '@angular/core';
import { EChartsType, EChartsOption, init } from 'echarts';

import { EchartService } from './echart.service';

@Component({
    selector: 'lib-echart',
    template: '<div class="echart" #echart>App Echart works!</div>',
    styles: [`
        :host { display: block; }
        .echart { width: 100%; height: 100%; }
    `]
})
export class EchartComponent implements AfterViewInit, OnDestroy {

    @Input()
    public config!: string;

    @ViewChild('echart')
    protected chartElRef!: ElementRef<HTMLDivElement>;

    private echart!: EChartsType;

    private rb = new ResizeObserver(entires => {
        this.zone.runOutsideAngular(() => {
            this.echart?.resize();
        });
    });

    constructor(private vm: EchartService, private zone: NgZone) { }

    public async ngAfterViewInit(): Promise<void> {
        this.initChart();
        this.rb.observe(this.chartElRef.nativeElement);
        void this.updateChartFromConfig();
    }

    public ngOnDestroy(): void {
        this.rb.unobserve(this.chartElRef.nativeElement);
    }

    private initChart(): void {
        const columns = [];
        for (let i = 0; i < 10; i++) {
            columns.push({
                type: 'rect',
                x: i * 20,
                shape: { x: 0, y: -40, width: 10, height: 80 },
                style: { fill: '#5470c6' },
                keyframeAnimation: {
                    duration: 1000,
                    delay: i * 200,
                    loop: true,
                    keyframes: [
                        { percent: 0.5, scaleY: 0.1, easing: 'cubicIn' },
                        { percent: 1, scaleY: 1, easing: 'cubicOut' }
                    ]
                }
            });
        }
        const initOpts: EChartsOption = {
            graphic: {
                elements: [
                    {
                        type: 'group',
                        left: 'center',
                        top: 'center',
                        children: columns as any[]
                    }
                ]
            }
        };
        this.echart = init(this.chartElRef.nativeElement);
        this.echart.setOption(initOpts);
    }

    private async updateChartFromConfig(): Promise<void> {
        const props = await this.vm.loadConfig(this.config);
        const opts = props.echarts;
        const result = await this.vm.loadData(props.data.url);
        opts.dataset = { source: result.data };
        this.zone.runOutsideAngular(() => {
            this.echart.clear();
            this.echart.resize();
            if (!!props.beforeSetChartOptions) {
                props.beforeSetChartOptions(opts);
            }
            this.echart.setOption(opts);
        });
    }

}

