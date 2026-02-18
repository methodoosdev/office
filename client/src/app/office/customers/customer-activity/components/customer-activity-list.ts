import { Component, Input } from "@angular/core";
import { CustomerActivityUnitOfWork } from "@officeNg";

@Component({
    selector: "customer-activity-list",
    templateUrl: "./customer-activity-list.html"
})
export class CustomerActivityListComponent {
    pathUrl = 'office/customer-activity';
    constructor(public uow: CustomerActivityUnitOfWork) { }
}
