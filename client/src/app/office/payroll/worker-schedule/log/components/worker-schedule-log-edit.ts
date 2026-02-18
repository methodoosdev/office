import { Component, ViewChild } from "@angular/core";
import { WorkerScheduleLogUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "worker-schedule-log-edit",
    templateUrl: "./worker-schedule-log-edit.html"
})
export class WorkerScheduleLogEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/worker-schedule-log';

    constructor(public uow: WorkerScheduleLogUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
