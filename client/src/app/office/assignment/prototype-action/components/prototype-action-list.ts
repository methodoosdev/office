import { Component } from "@angular/core";
import { AssignmentPrototypeActionUnitOfWork } from "@officeNg";

@Component({
    selector: "assignment-prototype-action-list",
    templateUrl: "./prototype-action-list.html"
})
export class AssignmentPrototypeActionListComponent {
    pathUrl = 'office/assignment-prototype-action';
    constructor(public uow: AssignmentPrototypeActionUnitOfWork) { }
 }
