import { Component } from "@angular/core";
import { EducationUnitOfWork } from "@officeNg";

@Component({
    selector: "education-list",
    templateUrl: "./education-list.html"
})
export class EducationListComponent {
    pathUrl = 'office/education';
    constructor(public uow: EducationUnitOfWork) { }
}
