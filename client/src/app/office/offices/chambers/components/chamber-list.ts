import { Component } from "@angular/core";
import { ChamberUnitOfWork } from "@officeNg";

@Component({
    selector: "chamber-list",
    templateUrl: "./chamber-list.html"
})
export class ChamberListComponent {
    pathUrl = 'office/chamber';
    constructor(public uow: ChamberUnitOfWork) { }
}
