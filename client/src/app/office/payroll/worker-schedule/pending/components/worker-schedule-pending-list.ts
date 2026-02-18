import { Component, ViewChild } from "@angular/core";
import { GridViewToken, ISearchModel, WorkerSchedulePendingUnitOfWork } from "@officeNg";
import { TranslationService } from "@core";

@Component({
    selector: "worker-schedule-pending-list",
    templateUrl: "./worker-schedule-pending-list.html"
})
export class WorkerSchedulePendingListComponent {
    @ViewChild(GridViewToken) table: GridViewToken | null = null;
    pathUrl = 'office/worker-schedule-pending';
    refreshLabel: string;
    searchModel: ISearchModel;

    constructor(
        private translationService: TranslationService,
        public uow: WorkerSchedulePendingUnitOfWork) {

        this.refreshLabel = this.translationService.translate('common.refresh');
    }

    refresh() {
        this.table.refresh();
    }
 }
