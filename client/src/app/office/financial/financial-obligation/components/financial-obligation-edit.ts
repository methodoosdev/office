import { Component, ViewChild } from "@angular/core";
import { FinancialObligationUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "financial-obligation",
    templateUrl: "./financial-obligation-edit.html"
})
export class FinancialObligationEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/financial-obligation';

    constructor(public uow: FinancialObligationUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
