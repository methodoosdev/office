import { Component, ViewChild } from "@angular/core";
import { SpecialtyUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "specialty-edit",
    templateUrl: "./specialty-edit.html"
})
export class SpecialtyEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/specialty';

    constructor(public uow: SpecialtyUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
