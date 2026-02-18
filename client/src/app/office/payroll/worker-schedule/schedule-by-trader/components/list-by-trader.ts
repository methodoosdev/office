import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { ColumnButtonClickEvent, WorkerScheduleByTraderUnitOfWork } from "@officeNg";

@Component({
    selector: "worker-schedule-list-by-trader",
    templateUrl: "./list-by-trader.html"
})
export class WorkerScheduleByTraderListComponent {
    pathUrl = 'office/worker-schedule-by-trader';

    constructor(
        private router: Router,
        public uow: WorkerScheduleByTraderUnitOfWork) {
    }

    columnButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'submit') {
            this.router.navigate(['office/worker-schedule-submit', event.dataItem.id]);
        }
        if (event.action === 'check') {
            this.router.navigate(['office/worker-schedule-check', event.dataItem.id]);
        }
    }
 }
