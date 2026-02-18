import { Component } from "@angular/core";
import { SimpleTaskSectorUnitOfWork } from "@officeNg";

@Component({
    selector: "simple-task-sector-list",
    templateUrl: "./simple-task-sector-list.html"
})
export class SimpleTaskSectorListComponent {
    pathUrl = 'office/simple-task-sector';
    constructor(public uow: SimpleTaskSectorUnitOfWork) { }
 }
