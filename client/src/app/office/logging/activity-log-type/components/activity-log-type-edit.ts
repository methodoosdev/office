import { Component, ViewChild } from "@angular/core";
import { ActivityLogTypeUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "activity-log-type-edit",
    templateUrl: "./activity-log-type-edit.html"
})
export class ActivityLogTypeEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/activity-log-type';

    constructor(public uow: ActivityLogTypeUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
