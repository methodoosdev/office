import { Component } from "@angular/core";
import { PeriodicityItemUnitOfWork } from "@officeNg";

@Component({
    selector: "periodicity-item-list",
    templateUrl: "./periodicity-item-list.html"
})
export class PeriodicityItemListComponent {
    pathUrl = 'office/periodicity-item';
    constructor(public uow: PeriodicityItemUnitOfWork) { }
}
