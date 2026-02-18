import { Component, OnInit } from "@angular/core";
import { AggregateDescriptor, GroupDescriptor, groupBy, GroupResult } from "@progress/kendo-data-query";

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { TraderRatingReportUnitOfWork } from "@officeNg";

@Component({
    selector: "trader-rating-by-trader",
    templateUrl: "./list.html",
    animations: [stateAnimation]
})
export class TraderRatingByTraderComponent implements OnInit {
    animate: boolean = true;
    data: any[];
    gridData: GroupResult[];
    columns: any[];
    toggleValue: boolean;

    aggregates: AggregateDescriptor[] = [
        { field: "value", aggregate: "sum" }
    ];
    groups: GroupDescriptor[];
    byEmployee: GroupDescriptor[] = [{ field: 'employee', aggregates: this.aggregates }];
    byTrader: GroupDescriptor[] = [{ field: 'trader', aggregates: this.aggregates }];

    title: string;
    autofitColumnsLabel: string;

    constructor(
        private uow: TraderRatingReportUnitOfWork,
        private translationService: TranslationService) {

        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    ngOnInit(): void {
        this.uow.byTrader()
            .then((result: any) => {

                this.title = result.title;
                this.columns = result.columns;
                this.data = result.data;
                this.toggleValue = true;

                this.loadData();

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.data && this.data.length > 0;
    }

    loadData() {
        this.groups = this.toggleValue ? this.byTrader : this.byEmployee;
        this.gridData = groupBy(this.data, this.groups);
    }

    toggle() {
        this.toggleValue = !this.toggleValue;
        if (this.toggleValue) {
            this.columns.find((col) => col.field === 'employee').hidden = false;
            this.columns.find((col) => col.field === 'trader').hidden = true;
        } else {
            this.columns.find((col) => col.field === 'trader').hidden = false;
            this.columns.find((col) => col.field === 'employee').hidden = true;
        }
        this.loadData();
    }

    onVisibilityChange(e: any): void {
        e.columns.forEach((column: any) => {
            this.columns.find((col) => col.field === column.field).hidden = column.hidden;
        });
    }

}
