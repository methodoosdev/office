import { Component, OnInit, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";

import { GridComponent } from "@progress/kendo-angular-grid";
import { groupBy, GroupDescriptor, GroupResult } from "@progress/kendo-data-query";

import { stateAnimation } from "@primeNg";
import { PayoffLiabilitiesUnitOfWork } from '@officeNg';
import { TranslationService } from "@core";

@Component({
    selector: "payoff-liabilities",
    templateUrl: "./payoff-liabilities.html",
    animations: [stateAnimation]
})
export class PayoffLiabilitiesComponent implements OnInit {
    @ViewChild(GridComponent, { static: false }) table: GridComponent;
    animate: boolean = true;
    groups: GroupDescriptor[] = [{ field: "payoffLiabilityCategoryTypeName" }];
    gridData: GroupResult[];
    columns: any[];

    factorGridData: any[];
    factorColumns: any[];
    factorTitle: string;

    title: string;
    exportToExcelLabel: string;
    exportToPdfLabel: string;
    calcLabel: string;
    traderLabel: string;
    periodLabel: string;
    autofitColumnsLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    constructor(
        private uow: PayoffLiabilitiesUnitOfWork,
        private translationService: TranslationService) {
        this.calcLabel = this.translationService.translate('common.calc');
        this.traderLabel = this.translationService.translate('common.trader');
        this.periodLabel = this.translationService.translate('common.period');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    modelChangeEvent(value: any) {
        this.gridData = null;
    }

    ngOnInit(): void {

        this.uow.loadProperties()
            .then((data: any) => {
                data.searchModel['period'] = new Date(data.searchModel['period']);

                this.factorTitle = data.factorTableModel.customProperties.title;
                this.factorColumns = data.factorTableModel.customProperties.columns;

                this.title = data.tableModel.customProperties.title;
                this.columns = data.tableModel.customProperties.columns;

                this.searchFields = data.searchModel.customProperties.fields;
                this.searchModel = data.searchModel;

            }).catch((err: any) => {
                throw err;
            });

    }

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((result: any) => {
                this.gridData = groupBy(result.list, this.groups); 
                this.factorGridData = result.factorList;
            }).catch((err: any) => {
                throw err;
            });
    }

    get canFactorExport() {
        return this.factorGridData && this.factorGridData.length > 0;
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    get canCalc() {
        return this.searchModel && this.searchModel.traderId > 0;
    }

}
