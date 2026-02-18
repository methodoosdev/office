import { Component } from "@angular/core";
import { ToastrService } from "ngx-toastr";

import { ColumnButtonClickEvent, ScheduleTaskUnitOfWork } from "@officeNg";
import { TranslationService } from "@core";

@Component({
    selector: "schedule-task-list",
    templateUrl: "./schedule-task-list.html"
})
export class ScheduleTaskListComponent {
    pathUrl = 'office/schedule-task';

    constructor(private translationService: TranslationService,
        public uow: ScheduleTaskUnitOfWork,
        private toastrService: ToastrService) {
    }

    columnButtonClick(event: ColumnButtonClickEvent) {
        if (event.action === 'runNow') {

            this.uow.runNow(event.dataItem.id)
                .then(() => {
                    this.toastrService.success(this.translationService.translate('message.successfullyCompleted'));
                }).catch((err: Error) => {
                    throw err;
                });
        }
    }
}
