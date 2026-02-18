import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { DomSanitizer, SafeStyle } from "@angular/platform-browser";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { stateAnimation } from "@primeNg";
import { VatTransferenceUnitOfWork } from '@officeNg';
import { ApiConfigService,TranslationService } from "@core";

@Component({
    selector: "vat-transference",
    templateUrl: "./vat-transference.html",
    animations: [stateAnimation]
})

export class VatTransferenceComponent implements OnInit {
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    calcLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    constructor(
        private uow: VatTransferenceUnitOfWork,
        private toastrService: ToastrService,
        private sanitizer: DomSanitizer,
        private translationService: TranslationService,
        private api: ApiConfigService) {

        this.title = this.translationService.translate('menu.vatTransference');
        this.calcLabel = this.translationService.translate('common.calc');
        this.traderLabel = this.translationService.translate('common.trader');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    get userIsTrader() {
        return !!this.api.configuration.trader;
    }

    ngOnInit(): void {

        this.uow.loadProperties()
            .then((data: any) => {
                this.title = data.tableModel.customProperties.title;
                this.columns = data.tableModel.customProperties.columns;

                this.searchFields = data.searchModel.customProperties.fields;
                this.searchModel = data.searchModel;

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((data: any) => {
                this.gridData = data;
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });    
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canCalc() {
        return this.searchModel && this.searchModel.traderId > 0;
    }

    public colorCode(value: number): SafeStyle {
        const result = value >= 0 ? '#424242' : '#ff0000';
        return this.sanitizer.bypassSecurityTrustStyle(result);
    }
}

