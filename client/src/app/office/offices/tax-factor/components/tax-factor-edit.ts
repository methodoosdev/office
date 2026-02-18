import { Component, ViewChild } from "@angular/core";
import { TaxFactorUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "tax-factor-edit",
    templateUrl: "./tax-factor-edit.html"
})
export class TaxFactorEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/tax-factor';

    constructor(public uow: TaxFactorUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
