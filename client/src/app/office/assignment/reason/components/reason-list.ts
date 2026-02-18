import { Component } from "@angular/core";
import { AssignmentReasonUnitOfWork } from "@officeNg";

@Component({
    selector: "assignment-reason-list",
    templateUrl: "./reason-list.html"
})
export class AssignmentReasonListComponent {
    pathUrl = 'office/assignment-reason';
    constructor(public uow: AssignmentReasonUnitOfWork) { }
 }
