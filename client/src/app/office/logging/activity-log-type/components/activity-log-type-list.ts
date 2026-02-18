import { Component } from "@angular/core";
import { ActivityLogTypeUnitOfWork } from "@officeNg";

@Component({
    selector: "activity-log-type-list",
    templateUrl: "./activity-log-type-list.html"
})
export class ActivityLogTypeListComponent {
    pathUrl = 'office/activity-log-type';
    constructor(public uow: ActivityLogTypeUnitOfWork) { }
}
