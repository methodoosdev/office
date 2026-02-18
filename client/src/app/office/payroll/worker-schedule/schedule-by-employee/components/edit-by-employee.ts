import { Component, ViewChild } from "@angular/core";
import { WorkerScheduleByEmployeeUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "worker-schedule-edit-by-employee",
    templateUrl: "./edit-by-employee.html"
})
export class WorkerScheduleByEmployeeEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/worker-schedule-by-employee';

    constructor(public uow: WorkerScheduleByEmployeeUnitOfWork) {
    }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
