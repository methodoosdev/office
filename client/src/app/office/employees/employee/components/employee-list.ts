import { Component } from "@angular/core";
import { EmployeeUnitOfWork } from "@officeNg";

@Component({
    selector: "employee-list",
    templateUrl: "./employee-list.html"
})
export class EmployeeListComponent {
    pathUrl = 'office/employee';
    constructor(public uow: EmployeeUnitOfWork) { }
}
