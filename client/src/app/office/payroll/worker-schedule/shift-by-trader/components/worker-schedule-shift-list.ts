import { Component } from "@angular/core";
import { WorkerScheduleShiftByTraderUnitOfWork } from "@officeNg";

@Component({
    selector: "worker-schedule-shift-list",
    templateUrl: "./worker-schedule-shift-list.html"
})
export class WorkerScheduleShiftListComponent {
    pathUrl = 'office/worker-schedule-shift-by-trader';
    constructor(public uow: WorkerScheduleShiftByTraderUnitOfWork) { }
 }
