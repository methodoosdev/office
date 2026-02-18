import { Component, ViewChild } from "@angular/core";
import { SimpleTaskNatureUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "simple-task-nature-edit",
    templateUrl: "./simple-task-nature-edit.html"
})
export class SimpleTaskNatureEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/simple-task-nature';

    constructor(public uow: SimpleTaskNatureUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
