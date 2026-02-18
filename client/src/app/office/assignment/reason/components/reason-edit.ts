import { Component, ViewChild } from "@angular/core";
import { AssignmentReasonUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "assignment-reason-edit",
    templateUrl: "./reason-edit.html"
})
export class AssignmentReasonEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/assignment-reason';

    constructor(public uow: AssignmentReasonUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
