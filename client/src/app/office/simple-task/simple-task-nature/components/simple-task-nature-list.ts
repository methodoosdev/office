import { Component } from "@angular/core";
import { SimpleTaskNatureUnitOfWork } from "@officeNg";

@Component({
    selector: "simple-task-nature-list",
    templateUrl: "./simple-task-nature-list.html"
})
export class SimpleTaskNatureListComponent {
    pathUrl = 'office/simple-task-nature';
    constructor(public uow: SimpleTaskNatureUnitOfWork) { }
 }
