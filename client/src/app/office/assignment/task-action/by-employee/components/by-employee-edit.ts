import { Component, ViewChild } from "@angular/core";
import { AssignmentTaskActionByEmployeeUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "assignment-task-action-by-employee-edit",
    templateUrl: "./by-employee-edit.html"
})
export class AssignmentTaskActionByEmployeeEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/assignment-task-action-by-employee';

    constructor(public uow: AssignmentTaskActionByEmployeeUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
