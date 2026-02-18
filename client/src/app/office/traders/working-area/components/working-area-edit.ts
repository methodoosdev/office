import { Component, ViewChild } from "@angular/core";
import { WorkingAreaUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "working-area-edit",
    templateUrl: "./working-area-edit.html"
})
export class WorkingAreaEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/working-area';

    constructor(public uow: WorkingAreaUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
