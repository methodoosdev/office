import { Component } from "@angular/core";
import { AccountingWorkUnitOfWork } from "@officeNg";

@Component({
    selector: "accounting-work-list",
    templateUrl: "./accounting-work-list.html"
})
export class AccountingWorkListComponent {
    pathUrl = 'office/accounting-work';
    constructor(public uow: AccountingWorkUnitOfWork) { }
}
