import { Component, ViewChild } from "@angular/core";
import { AccountingWorkUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "accounting-work-edit",
    templateUrl: "./accounting-work-edit.html"
})
export class AccountingWorkEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/accounting-work';

    constructor(public uow: AccountingWorkUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
