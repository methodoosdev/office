import { Component } from "@angular/core";
import { CustomerOnlineUnitOfWork } from "@officeNg";

@Component({
    selector: "customer-online-list",
    templateUrl: "./customer-online-list.html"
})
export class CustomerOnlineListComponent {
    pathUrl = 'office/customer-online';
    constructor(public uow: CustomerOnlineUnitOfWork) { }
}
