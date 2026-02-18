import { Component } from "@angular/core";
import { WorkerLeaveDetailUnitOfWork } from "@officeNg";

@Component({
    selector: "leave-detail-list",
    templateUrl: "./leave-detail-list.html"
})
export class WorkerLeaveDetailListComponent {
    pathUrl = 'office/worker-leave-detail';
    constructor(public uow: WorkerLeaveDetailUnitOfWork) { }
}

