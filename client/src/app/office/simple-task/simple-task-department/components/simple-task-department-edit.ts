import { Component, ViewChild } from "@angular/core";
import { SimpleTaskDepartmentUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "simple-task-department-edit",
    templateUrl: "./simple-task-department-edit.html"
})
export class SimpleTaskDepartmentEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/simple-task-department';

    constructor(public uow: SimpleTaskDepartmentUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
