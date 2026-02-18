import { Component } from "@angular/core";
import { JobTitleUnitOfWork } from "@officeNg";

@Component({
    selector: "jobTitle-list",
    templateUrl: "./jobTitle-list.html"
})
export class JobTitleListComponent {
    pathUrl = 'office/jobTitle';
    constructor(public uow: JobTitleUnitOfWork) { }
}
