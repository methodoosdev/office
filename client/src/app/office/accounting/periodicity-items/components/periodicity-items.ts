import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { saveAs } from "@progress/kendo-file-saver";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { TranslationService } from "@core";
import { PeriodicityItemsUnitOfWork } from "@officeNg";

@Component({
    selector: "periodicity-items",
    templateUrl: "./periodicity-items.html",
    animations: [stateAnimation]
})
export class PeriodicityItemsComponent implements OnInit {
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    calcLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    constructor(
        private uow: PeriodicityItemsUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.calcLabel = this.translationService.translate('common.calc');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    ngOnInit(): void {
        this.uow.loadProperties()
            .then((result: any) => {
                result.searchModel['period'] = new Date(result.searchModel['period']);

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
            .then((result) => {
                if (result)
                    saveAs(result, "periodicityItems.pdf");
                else
                    this.toastrService.error('Pdf file cannot be created.');
            }).catch((err: Error) => {
                throw err;
            });
    }
}
