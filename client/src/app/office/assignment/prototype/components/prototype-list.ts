import { Component } from "@angular/core";
import { AssignmentPrototypeUnitOfWork } from "@officeNg";

@Component({
    selector: "assignment-prototype-list",
    templateUrl: "./prototype-list.html"
})
export class AssignmentPrototypeListComponent {
    pathUrl = 'office/assignment-prototype';
    constructor(public uow: AssignmentPrototypeUnitOfWork) { }
 }
