import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { ConfiguratorSettings, Dimension, Measure, PivotGridAxis, sumAggregate } from '@progress/kendo-angular-pivotgrid';
import { IntlService } from "@progress/kendo-angular-intl";

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { TraderRatingReportUnitOfWork } from "@officeNg";
import { TraderRatingItem } from "../common/trader-rating-item";

@Component({
    selector: "trader-rating-by-department",
    templateUrl: "./list.html",
    animations: [stateAnimation],
    encapsulation: ViewEncapsulation.None,
    styles: [
        `
      .k-pivotgrid-row-headers table.k-pivotgrid-table {
          table-layout: auto;
      }

      .k-pivotgrid-table .k-pivotgrid-header-root {
          text-transform: none !important;
      }

      .k-pivotgrid-values .k-pivotgrid-row {
          text-align: right;
      }
    `,
    ]
})
export class TraderRatingByDepartmentComponent implements OnInit {
    animate: boolean = true;
    gridData: TraderRatingItem[];
    columns: any[];

    title: string;
    autofitColumnsLabel: string;
    totalLabel: string = this.translationService.translate('common.total');
    tradersLabel: string = this.translationService.translate('menu.traders');
    departmentsLabel: string = this.translationService.translate('menu.departments');
    employeesLabel: string = this.translationService.translate('menu.employees');

    constructor(
        private intl: IntlService,
        private uow: TraderRatingReportUnitOfWork,
        private translationService: TranslationService) {

        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    formatNumber(value: number) {
        return value ? this.intl.formatNumber(value, "n0") : undefined;
    }

    ngOnInit(): void {
        this.uow.byDepartment()
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

    exportToPdf() {
    }

    settings: ConfiguratorSettings = {
        position: "left",
        orientation: "vertical",
    };

    dimensions: { [key: string]: Dimension } = {
        department: {
            caption: this.departmentsLabel,
            displayValue: (item) => item.department,
            sortValue: (displayValue: string) => displayValue
        },
        //category: {
        //    caption: "Categories",
        //    displayValue: (item) => item.category,
        //    sortValue: (displayValue: string) => displayValue
        //},
        //gravity: {
        //    caption: "Gravities",
        //    displayValue: (item) => item.gravity,
        //    sortValue: (displayValue: string) => displayValue
        //},
        //employee: {
        //    caption: this.employeesLabel,
        //    displayValue: (item) => item.employee,
        //    sortValue: (displayValue: string) => displayValue
        //},
        trader: {
            caption: this.tradersLabel,
            displayValue: (item) => item.trader,
            sortValue: (displayValue: string) => displayValue
        }
    };

    measures: Measure[] = [
        { name: this.totalLabel, value: (item: TraderRatingItem): number => item.value, aggregate: sumAggregate }
    ];

    defaultMeasureAxes: PivotGridAxis[] = [{ name: [this.totalLabel] }];

    defaultRowAxes: PivotGridAxis[] = [
        { name: ["trader"], expand: true }
    ];

    defaultColumnAxes: PivotGridAxis[] = [
        { name: ["department"], expand: true }
    ];
}
