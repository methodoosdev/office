import { Component } from "@angular/core";
import { SimpleTaskDepartmentUnitOfWork } from "@officeNg";

@Component({
    selector: "simple-task-department-list",
    templateUrl: "./simple-task-department-list.html"
})
export class SimpleTaskDepartmentListComponent {
    pathUrl = 'office/simple-task-department';
    constructor(public uow: SimpleTaskDepartmentUnitOfWork) { }
 }
