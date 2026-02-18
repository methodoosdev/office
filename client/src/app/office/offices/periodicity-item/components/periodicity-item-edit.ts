import { Component, ViewChild } from "@angular/core";
import { PeriodicityItemUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "periodicity-item-edit",
    templateUrl: "./periodicity-item-edit.html"
})
export class PeriodicityItemEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/periodicity-item';

    constructor(public uow: PeriodicityItemUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
