import { Component } from "@angular/core";
import { BankingTraderUnitOfWork } from "@officeNg";

@Component({
    selector: "banking-trader-list",
    templateUrl: "./trader-list.html"
})
export class BankingTraderListComponent {
    pathUrl = 'office/banking-trader';
    constructor(public uow: BankingTraderUnitOfWork) { }
}
