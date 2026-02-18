import { Component } from "@angular/core";
import { TaxFactorUnitOfWork } from "@officeNg";

@Component({
    selector: "tax-factor-list",
    templateUrl: "./tax-factor-list.html"
})
export class TaxFactorListComponent {
    pathUrl = 'office/tax-factor';
    constructor(public uow: TaxFactorUnitOfWork) { }
}
