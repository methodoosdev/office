import { Component, OnInit, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { saveAs } from '@progress/kendo-file-saver';
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";

import { ApiConfigService } from "@core";
import { stateAnimation } from "@primeNg";
import { TranslationService } from "@core";
import { CountingDocumentUnitOfWork } from "@officeNg";
import { AggregateDescriptor, GroupDescriptor, groupBy } from "@progress/kendo-data-query";
import { GridComponent, GroupKey } from "@progress/kendo-angular-grid";

@Component({
    selector: "counting-document",
    templateUrl: "./counting-document.html",
    animations: [stateAnimation]
})
export class CountingDocumentComponent implements OnInit {
    @ViewChild(GridComponent, { static: false }) table: GridComponent;
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    calcLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;
    collapseLabel: string;
    expandLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    initiallyExpanded = true;
    expandedGroupKeys: GroupKey[] = [];

    aggregates: AggregateDescriptor[] = [
        { field: "recCount", aggregate: "sum" },
    ];

    group: GroupDescriptor[] = [
        { field: "docType", aggregates: this.aggregates },
    ];

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    constructor(
        private uow: CountingDocumentUnitOfWork,
        private api: ApiConfigService,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
            
        this.calcLabel = this.translationService.translate('common.calc');
        this.traderLabel = this.translationService.translate('common.trader');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
        this.collapseLabel = this.translationService.translate('common.collapse');
        this.expandLabel = this.translationService.translate('common.expand');
    }

    get userIsTrader() {
        return !!this.api.configuration.trader;
    }

    ngOnInit(): void {
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

    public toggleGroups(): void {
        this.expandedGroupKeys = [];
        this.initiallyExpanded = !this.initiallyExpanded;
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
                this.gridData = groupBy(result.data, this.group);
                
                setTimeout(() => {
                    this.table.autoFitColumns();
                }, 10);
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }
}
