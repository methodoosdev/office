import { Component, ViewChild } from "@angular/core";
import { ScheduleTaskUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "schedule-task-edit",
    templateUrl: "./schedule-task-edit.html"
})
export class ScheduleTaskEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/schedule-task';

    constructor(public uow: ScheduleTaskUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
