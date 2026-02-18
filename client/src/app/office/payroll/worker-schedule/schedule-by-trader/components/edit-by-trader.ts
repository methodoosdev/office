import { Component, ViewChild } from "@angular/core";
import { WorkerScheduleByTraderUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "worker-schedule-edit-by-trader",
    templateUrl: "./edit-by-trader.html"
})
export class WorkerScheduleByTraderEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/worker-schedule-by-trader';

    constructor(public uow: WorkerScheduleByTraderUnitOfWork) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
