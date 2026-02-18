import { Component, ViewChild } from "@angular/core";

import { CanComponentDeactivate } from "@jwtNg";
import { AfterModelChangeEvent, FormEditToken, WorkerScheduleShiftByTraderUnitOfWork } from "@officeNg";
import { Observable } from "rxjs";

@Component({
    selector: "worker-schedule-shift-edit",
    templateUrl: "./worker-schedule-shift-edit.html"
})
export class WorkerScheduleShiftEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/worker-schedule-shift-by-trader';
    minDate: Date = new Date(2000, 2, 2, 0, 0, 0);

    constructor(public uow: WorkerScheduleShiftByTraderUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }

    afterModelChange(e: AfterModelChangeEvent) {
        setTimeout(() => {

            e.form.get('isSplit').valueChanges.subscribe((isSplit: boolean) => {
                if (!isSplit) {
                    const properties: string[] = ['splitFromDate', 'splitToDate', 'breakSplitFromDate', 'breakSplitToDate'];

                    properties.forEach((prop) => {
                        e.form.get(prop).setValue(this.minDate);
                    });
                }
            });
        }, 0);
    }

}
