import { Component, ViewChild } from "@angular/core";
import { CanComponentDeactivate } from "@jwtNg";
import { AssignmentPrototypeActionUnitOfWork, FormEditToken } from "@officeNg";
import { Observable } from "rxjs";

@Component({
    selector: "assignment-prototype-action-edit",
    templateUrl: "./prototype-action-edit.html"
})
export class AssignmentPrototypeActionEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/assignment-prototype-action';

    constructor(public uow: AssignmentPrototypeActionUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
