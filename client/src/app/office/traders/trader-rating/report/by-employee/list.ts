import { Component, OnInit } from "@angular/core";

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { TraderRatingReportUnitOfWork } from "@officeNg";

@Component({
    selector: "trader-rating-by-employee",
    templateUrl: "./list.html",
    animations: [stateAnimation]
})
export class TraderRatingByEmployeeComponent implements OnInit {
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    autofitColumnsLabel: string;

    constructor(
        private uow: TraderRatingReportUnitOfWork,
        private translationService: TranslationService) {

        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    ngOnInit(): void {
        this.uow.byEmployee()
            .then((result: any) => {

                this.title = result.title;
                this.columns = result.columns;
                this.gridData = result.data;

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }
}
