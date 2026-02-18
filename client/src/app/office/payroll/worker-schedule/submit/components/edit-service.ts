import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, of } from 'rxjs';

export interface WorkerScheduleDate {
    workerScheduleId: number;
    id: number;
    traderId: number;
    workerId: number;
    workerVat: string;
    workingDate: Date;
    nonstopFromDate: Date;
    nonstopToDate: Date;
    breakNonstopFromDate: Date;
    breakNonstopToDate: Date;
    splitFromDate: Date;
    splitToDate: Date;
    breakSplitFromDate: Date;
    breakSplitToDate: Date;
    overtimeFromDate: Date;
    overtimeToDate: Date;
    overtimeTypeId: number;
    leave: boolean;
    sickLeave: boolean;
    weekOfYear: number;
    isSaturday: boolean;
    isSunday: boolean;
    breakNonstop2FromDate: Date;
    breakNonstop2ToDate: Date;
    isSplit: boolean;
    //
    overtimeTypeName: string;
    workerCardName: string;
    workerName: string;
    dailyNonstop: number;
    dailySplit: number;
    dailyBreak: number;
    dailyTotalHours: number;
}

@Injectable()
export class EditService extends BehaviorSubject<WorkerScheduleDate[]> {
    originalData: WorkerScheduleDate[] = [];
    data: WorkerScheduleDate[] = [];
    private minDate: Date = new Date(2000, 2, 2, 0, 0, 0);

    constructor() {
        super([]);
    }

    public purgeData(data: any[], columns: any) {
        data.forEach((model) => {

            Object.keys(columns).forEach(key => {
                const property = columns[key];

                if (property['fieldType'] === 'time' || property['fieldType'] === 'date') {
                    const date = Date.parse(model[key]);
                    if (!isNaN(date)) {
                        model[key] = new Date(date);
                    }
                }
            });
        });
    }

    private setTotal(x: WorkerScheduleDate): void {
        let dailyBreakNonstop = x.breakNonstopToDate.getTime() - x.breakNonstopFromDate.getTime();
        let dailyBreakNonstop2 = x.breakNonstop2ToDate.getTime() - x.breakNonstop2FromDate.getTime();
        let dailyBreakSplit = x.breakSplitToDate.getTime() - x.breakSplitFromDate.getTime();
        let dailyNonstop = x.nonstopToDate.getTime() - x.nonstopFromDate.getTime();
        let dailySplit = x.splitToDate.getTime() - x.splitFromDate.getTime();

        dailyBreakNonstop = dailyBreakNonstop < 0 ? (24 * 60 * 60000) + dailyBreakNonstop : dailyBreakNonstop;
        dailyBreakNonstop2 = dailyBreakNonstop2 < 0 ? (24 * 60 * 60000) + dailyBreakNonstop2 : dailyBreakNonstop2;
        dailyBreakSplit = dailyBreakSplit < 0 ? (24 * 60 * 60000) + dailyBreakSplit : dailyBreakSplit;
        dailyNonstop = dailyNonstop < 0 ? (24 * 60 * 60000) + dailyNonstop : dailyNonstop;
        dailySplit = dailySplit < 0 ? (24 * 60 * 60000) + dailySplit : dailySplit;

        const dailyBreak = dailyBreakNonstop + dailyBreakNonstop2 + dailyBreakSplit;
        const dailyTotalHours = dailyNonstop + dailySplit - dailyBreak;

        x.dailyNonstop = dailyNonstop;
        x.dailySplit = dailySplit;
        x.dailyBreak = dailyBreak;
        x.dailyTotalHours = dailyTotalHours;
    }

    private load(data: WorkerScheduleDate[]) {
        data.forEach((item) => {
            this.setTotal(item);
        });

        super.next(data);
    }

    public init(data: WorkerScheduleDate[]) {
        this.originalData = data;
    }

    public read(): void {
        if (this.data.length) {
            this.load(this.data)
            return;
        }

        this.fetch()
            .subscribe(data => {
                this.data = data;
                this.load(data);
            });
    }

    public save(dataItem: WorkerScheduleDate) {

        if (!dataItem) {
            throw new Error('WorkerScheduleDate: Not exit!');
        }

        // find orignal data item
        const originalDataItem = this.originalData.find(item => item.id === dataItem.id);

        // revert changes
        Object.assign(originalDataItem, dataItem);
        this.reset();

        this.fetch()
            .subscribe(() => this.read(), () => this.read());
    }

    public resetItem(dataItem: WorkerScheduleDate): void {
        if (!dataItem) { return; }

        // find orignal data item
        const originalDataItem = this.data.find(item => item.id === dataItem.id);

        // revert changes
        Object.assign(originalDataItem, dataItem);

        this.load(this.data)
    }

    private reset() {
        this.data = [];
    }

    private fetch(): Observable<WorkerScheduleDate[]> {
        return of(this.originalData)
    }

    private _properties: string[] = [
        'nonstopFromDate', 'nonstopToDate', 'splitFromDate', 'splitToDate', 'breakNonstop2FromDate', 'breakNonstop2ToDate',
        'breakNonstopFromDate', 'breakNonstopToDate', 'breakSplitFromDate', 'breakSplitToDate', 'overtimeFromDate', 'overtimeToDate'
    ];

    public resetDataItem(dataItem: any): void {
        this._properties.forEach((prop) => {
            dataItem[prop] = this.minDate;
        });
        dataItem['leave'] = false;
        dataItem['sickLeave'] = false;
        dataItem['isSplit'] = false;
        dataItem['dailyNonstop'] = 0;
        dataItem['dailySplit'] = 0;
        dataItem['dailyBreak'] = 0;
        dataItem['dailyTotalHours'] = 0;
    }

    onSelectedShiftChange(selectedKeys: any[], shift: any): WorkerScheduleDate[]  {
        const updateItems: WorkerScheduleDate[] = [];

        selectedKeys.forEach((id) => {
            const dataItem: any = this.data.find((x) => {
                return x.id === id;
            });

            const updated = Object.assign({}, dataItem);

            this._properties.forEach((prop) => {
                updated[prop] = new Date(Date.parse(shift[prop]));
            });
            updated['leave'] = false;
            updated['sickLeave'] = false;
            updated['isSplit'] = shift['isSplit'];

            updateItems.push(updated);
        });

        return updateItems;
    }

}