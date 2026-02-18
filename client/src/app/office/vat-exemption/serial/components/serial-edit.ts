import { Component, ViewChild } from "@angular/core";
import { VatExemptionSerialUnitOfWork, FormEditToken } from "@officeNg";
import { CanComponentDeactivate } from "@jwtNg";
import { Observable } from "rxjs";

@Component({
    selector: "vat-exemption-serial-edit",
    templateUrl: "./serial-edit.html"
})
export class VatExemptionSerialEditComponent implements CanComponentDeactivate {
    @ViewChild(FormEditToken) formEdit: FormEditToken | null = null;
    parentUrl = 'office/vat-exemption-serial';

    constructor(public uow: VatExemptionSerialUnitOfWork) { }

    canDeactivate(): boolean | Observable<boolean> | Promise<boolean> {
        return this.formEdit.canDeactivate();
    }
}
