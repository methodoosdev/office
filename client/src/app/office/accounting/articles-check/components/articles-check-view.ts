import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { DomSanitizer, SafeStyle } from "@angular/platform-browser";
import { saveAs } from '@progress/kendo-file-saver';

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { ArticlesCheckUnitOfWork } from "@officeNg";
import { ActivatedRoute } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { AggregateDescriptor, AggregateResult, DataResult, GroupDescriptor, process } from "@progress/kendo-data-query";

@Component({
    selector: "articles-check-view",
    templateUrl: "./articles-check-view.html",
    animations: [stateAnimation],
    encapsulation: ViewEncapsulation.None,
    styles: [
        `
            .articles-grid .k-grid .k-grid-header {
                display: none;
            }
        `
    ]
})
export class ArticlesCheckViewComponent implements OnInit {
    aggregates: AggregateDescriptor[] = [
        { field: "debit", aggregate: "sum" },
        { field: "credit", aggregate: "sum" }
    ];
    byType: GroupDescriptor[] = [{ field: 'group', aggregates: this.aggregates }];
    total1: AggregateResult;
    total2: AggregateResult;

    animate: boolean = true;
    gridData: DataResult;
    exportData: any[];
    columns: any[];

    title: string;
    traderName: string;
    exportLabel: string;
    autofitColumnsLabel: string;
    softOneConValid: boolean;
    databaseConnectionFailedLabel: string;

    grandTotalLabel: string;

    constructor(
        private route: ActivatedRoute,
        private uow: ArticlesCheckUnitOfWork,
        private sanitizer: DomSanitizer,
        private translationService: TranslationService,
        private toastrService: ToastrService) {
    }

    ngOnInit(): void {
        this.exportLabel = this.translationService.translate('common.export');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
        this.grandTotalLabel = this.translationService.translate('common.grandTotal');
        this.databaseConnectionFailedLabel = this.translationService.translate('message.databaseConnectionFailed');

        this.route.params.forEach((params: any) => {

            const companyId = +params.companyId;
            const nglId = +params.nglId;
            const year = +params.year;
            const period = +params.period;

            this.uow.view(companyId, nglId, year, period)
                .then((res: any) => {
                    this.title = res.tableModel.customProperties.title;
                    this.columns = res.tableModel.customProperties.columns;
                    this.softOneConValid = res.softOneConValid;
                    this.traderName = res.traderName;
                    this.exportData = res.data;

                    this.gridData = process(res.data, { group: this.byType });
                    this.total1 = this.gridData.data.reduce((accumulator, obj) => {
                        return accumulator + obj.aggregates.debit.sum;
                    }, 0);
                    this.total2 = this.gridData.data.reduce((accumulator, obj) => {
                        return accumulator + obj.aggregates.credit.sum;
                    }, 0);
                }).catch((e: any) => {
                    this.toastrService.error(e.error);
                });
        });
    }

    get canExport() {
        return this.gridData && this.gridData.data.length > 0;
    }

    public colorCode(dataItem: any, field: string): SafeStyle {
        const value = dataItem.formatValid === true && dataItem.schemaValid === true;
        const result = value ? '#424242' : '#ff0000';
        return this.sanitizer.bypassSecurityTrustStyle(result);
    }

    exportToExcel() {
        this.uow.exportToPdf(this.exportData)
            .then((result: any) => {
                saveAs(result, `${this.traderName.trim()}.xlsx`);

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

}
