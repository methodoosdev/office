import { Component, ViewChild } from "@angular/core";
import { SimpleTaskSectorUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "simple-task-sector-edit",
    templateUrl: "./simple-task-sector-edit.html"
})
export class SimpleTaskSectorEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/simple-task-sector';

    constructor(public uow: SimpleTaskSectorUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
