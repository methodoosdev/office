import { Component } from "@angular/core";
import { CustomerRoleUnitOfWork } from "@officeNg";

@Component({
    selector: "customer-role-list",
    templateUrl: "./customer-role-list.html"
})
export class CustomerRoleListComponent {
    pathUrl = 'office/customer-role';
    constructor(public uow: CustomerRoleUnitOfWork) { }
}
