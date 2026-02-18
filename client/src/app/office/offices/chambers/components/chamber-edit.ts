import { Component, ViewChild } from "@angular/core";
import { ChamberUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "chamber-edit",
    templateUrl: "./chamber-edit.html"
})
export class ChamberEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/chamber';

    constructor(public uow: ChamberUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
