import { Component } from "@angular/core";
import { DepartmentUnitOfWork } from "@officeNg";

@Component({
    selector: "department-list",
    templateUrl: "./department-list.html"
})
export class DepartmentListComponent {
    pathUrl = 'office/department';
    constructor(public uow: DepartmentUnitOfWork) { }
 }
