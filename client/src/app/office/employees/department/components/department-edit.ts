import { Component, ViewChild } from "@angular/core";
import { DepartmentUnitOfWork, FormlyEditNewToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "department-edit",
    templateUrl: "./department-edit.html"
})
export class DepartmentEditComponent implements CanComponentDeactivate {
    @ViewChild(FormlyEditNewToken, { static: true }) editForm: FormlyEditNewToken | null = null;
    parentUrl = 'office/department';

    constructor(public uow: DepartmentUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.editForm.canDeactivate();
    }
}
