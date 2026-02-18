import { Component } from "@angular/core";
import { SpecialtyUnitOfWork } from "@officeNg";

@Component({
    selector: "specialty-list",
    templateUrl: "./specialty-list.html"
})
export class SpecialtyListComponent {
    pathUrl = 'office/specialty';
    constructor(public uow: SpecialtyUnitOfWork) { }
}
