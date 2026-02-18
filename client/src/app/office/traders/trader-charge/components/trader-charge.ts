import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { saveAs } from '@progress/kendo-file-saver';
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";
import { AggregateDescriptor, AggregateResult, DataResult, GroupDescriptor, process } from "@progress/kendo-data-query";

import { stateAnimation } from "@primeNg";
import { TraderChargeUnitOfWork } from '@officeNg';
import { TranslationService } from "@core";

@Component({
    selector: "trader-charge",
    templateUrl: "./trader-charge.html",
    styles: [`
        .k-footer-template.k-table-row .k-table-td {
            text-align: right;
        }
    `],
    encapsulation: ViewEncapsulation.None,
    animations: [stateAnimation]
})
export class TraderChargeComponent implements OnInit {
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    calcLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    constructor(
        private uow: TraderChargeUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
    }

    ngOnInit(): void {
        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.calcLabel = this.translationService.translate('common.calc');
        this.traderLabel = this.translationService.translate('common.trader');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');

        this.uow.loadProperties()
            .then((result: any) => {
                this.title = result.tableModel.customProperties.title;
                this.columns = result.tableModel.customProperties.columns;

                this.searchFields = result.searchModel.customProperties.fields;
                this.searchModel = result.searchModel;

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canCalc() {
        const exist = typeof this.searchModel?.traderId === "number";
        return exist ? this.searchModel.traderId > 0 : false;
    }

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((result: any) => {
                this.gridData = result;
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    exportToPdf() {
        this.uow.exportToPdf(this.searchModel)
            .then((result: any) => {
                if (result)
                    saveAs(result, "traderCharge.pdf");
                else
                    this.toastrService.error('Pdf file cannot be created.');
            }).catch((err: Error) => {
                throw err;
            });
    }

    public getSum(field: string): number {

        if (["turnover", "beforeTaxes", "assets"].includes(field))
            return this.gridData.reduce((acc, item) => acc + (item[field] || 0), 0);

        return undefined;
    }
}
