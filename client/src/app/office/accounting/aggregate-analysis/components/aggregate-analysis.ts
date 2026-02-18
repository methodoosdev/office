import { Component, OnInit, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { saveAs } from '@progress/kendo-file-saver';
import { GridComponent } from "@progress/kendo-angular-grid";
import { ToastrService } from "ngx-toastr";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { getFieldProperties } from "@formlyNg";
import { AggregateAnalysisUnitOfWork } from "@officeNg";

@Component({
    selector: "aggregate-analysis",
    templateUrl: "./aggregate-analysis.html",
    animations: [stateAnimation]
})
export class AggregateAnalysisComponent implements OnInit {
    @ViewChild('table3', { static: false }) table3: GridComponent;
    animate: boolean = true;
    gridData: any[];
    progressData: any[];
    totalData: any[];
    columns: any[];
    totalColumns: any[];

    title: string;
    title1: string;
    title2: string;
    title3: string;
    calcLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    constructor(
        private uow: AggregateAnalysisUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.calcLabel = this.translationService.translate('common.calc');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    modelChangeEvent(value: any) {
        this.gridData = undefined;
        this.progressData = undefined;
        this.totalData = undefined;
    }

    ngOnInit(): void {
        this.uow.loadProperties()
            .then((result: any) => {

                this.title = result.tableModel.customProperties.title;
                this.title1 = result.tableModel.customProperties.title1;
                this.title2 = result.tableModel.customProperties.title2;
                this.title3 = result.tableModel.customProperties.title3;
                this.columns = result.tableModel.customProperties.columns;
                this.totalColumns = result.totalModel.customProperties.columns;

                this.searchFields = result.searchModel.customProperties.fields;
                this.searchModel = result.searchModel;

                const fieldProperties = getFieldProperties(this.searchFields);

                if (fieldProperties['traderId']) {
                    fieldProperties['traderId'].props['selectionChange'] = (field: FormlyFieldConfig, trader: any) => {

                        if (trader) {
                            this.uow.traderChanged(trader.id)
                                .then((result: any) => {
                                    field.form.get('year').setValue(result.year);
                                    field.form.get('period').setValue(result.period);
                                    fieldProperties['year'].props['options'] = result.years;
                                    fieldProperties['period'].props['options'] = result.periods;

                                }).catch((err) => {
                                    throw err;
                                });
                        } else {
                            const form = field.formControl.parent!;
                            form.get('year')!.setValue(null);
                            form.get('period')!.setValue(null);

                            fieldProperties['year'].props['options'] = [];
                            fieldProperties['period'].props['options'] = [];
                        }
                    };
                }

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canExport2() {
        return this.progressData && this.progressData.length > 0;
    }

    get canExport3() {
        return this.totalData && this.totalColumns.length > 0;
    }

    get canCalc() {
        const exist = typeof this.searchModel?.traderId === "number";
        return exist ? this.searchModel.traderId > 0 : false;
    }

    calc() {
        this.modelChangeEvent({});

        this.uow.loadDataSource(this.searchModel)
            .then((result: any) => {
                this.gridData = result.modelList;
                this.progressData = result.progressList;
                this.totalData = result.totalList;

                setTimeout(() => {
                    this.table3.autoFitColumns();
                }, 500);
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    exportToPdf() {

        this.uow.exportToPdf(this.searchModel)
            .then((result) => {
                if (result)
                    saveAs(result, "aggregateAnalysis.pdf");
                else
                    this.toastrService.error('Pdf file cannot be created.');
            }).catch((err: Error) => {
                throw err;
            });
    }
}
