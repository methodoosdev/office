import { Component, ViewChild } from "@angular/core";
import { AccountingOfficeUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "accounting-office-edit",
    templateUrl: "./accounting-office-edit.html"
})
export class AccountingOfficeEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office';

    constructor(public uow: AccountingOfficeUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
