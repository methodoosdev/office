import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { saveAs } from '@progress/kendo-file-saver';
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { getFieldProperties } from "@formlyNg";
import { TranslationService } from "@core";
import { VatCalculationUnitOfWork } from "@officeNg";

@Component({
    selector: "vat-calculation",
    templateUrl: "./vat-calculation.html",
    animations: [stateAnimation]
})
export class VatCalculationComponent implements OnInit {
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
        private uow: VatCalculationUnitOfWork,
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

                const fieldProperties = getFieldProperties(this.searchFields);

                if (fieldProperties['traderId']) {
                    fieldProperties['traderId'].props['selectionChange'] = (field: FormlyFieldConfig, trader: any) => {

                        if (trader) {
                            this.uow.traderChanged(trader.id)
                                .then((result: any) => {
                                    field.form.get('year').setValue(result.year);
                                    fieldProperties['year'].props['options'] = result.years;

                                }).catch((err) => {
                                    throw err;
                                });
                        } else {
                            const form = field.formControl.parent!;
                            form.get('year')!.setValue(null);
                            fieldProperties['year'].props['options'] = [];
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

    get canCalc() {
        const exist = typeof this.searchModel?.traderId === "number";
        return exist ? this.searchModel.traderId > 0 : false;
    }

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((result: any) => {
                this.gridData = result.data;
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    exportToPdf() {
        this.uow.exportToPdf(this.searchModel)
            .then((result: any) => {
                if (result)
                    saveAs(result, "vatCalculation.pdf");
                else
                    this.toastrService.error('Pdf file cannot be created.');
            }).catch((err: Error) => {
                throw err;
            });
    }
}
