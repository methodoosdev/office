import { Component, ViewChild } from "@angular/core";
import { SimpleTaskManagerUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "simple-task-manager-edit",
    templateUrl: "./simple-task-manager-edit.html"
})
export class SimpleTaskManagerEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/simple-task-manager';

    constructor(public uow: SimpleTaskManagerUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
