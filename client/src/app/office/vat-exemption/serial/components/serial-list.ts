import { Component } from "@angular/core";
import { VatExemptionSerialUnitOfWork } from "@officeNg";

@Component({
    selector: "vat-exemption-serial-list",
    templateUrl: "./serial-list.html"
})
export class VatExemptionSerialListComponent {
    pathUrl = 'office/vat-exemption-serial';
    constructor(public uow: VatExemptionSerialUnitOfWork) { }
 }
