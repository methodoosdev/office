import { Component } from "@angular/core";
import { WorkingAreaUnitOfWork } from "@officeNg";

@Component({
    selector: "working-area-list",
    templateUrl: "./working-area-list.html"
})
export class WorkingAreaListComponent {
    pathUrl = 'office/working-area';
    constructor(public uow: WorkingAreaUnitOfWork) { }
}
