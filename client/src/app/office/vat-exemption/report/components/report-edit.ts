import { Component, ViewChild } from "@angular/core";
import { VatExemptionReportUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "vat-exemption-report-edit",
    templateUrl: "./report-edit.html"
})
export class VatExemptionReportEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/vat-exemption-report';

    constructor(public uow: VatExemptionReportUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
