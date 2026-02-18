import { Component } from "@angular/core";
import { VatExemptionReportUnitOfWork } from "@officeNg";

@Component({
    selector: "vat-exemption-report-list",
    templateUrl: "./report-list.html"
})
export class VatExemptionReportListComponent {
    pathUrl = 'office/vat-exemption-report';
    constructor(public uow: VatExemptionReportUnitOfWork) { }
 }
