import { Component, OnInit, NgZone, ViewChild } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { saveAs } from "@progress/kendo-file-saver";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";
import { AggregateDescriptor, AggregateResult, DataResult, GroupDescriptor, process } from "@progress/kendo-data-query";

import { stateAnimation } from "@primeNg";
import { TranslationService } from "@core";
import { MonthlyBCategoryBulletinUnitOfWork } from "@officeNg";
import { getFieldProperties } from "@formlyNg";
import { GridComponent, GroupKey, GroupableSettings } from "@progress/kendo-angular-grid";
import { take } from "rxjs/operators";

@Component({
    selector: "monthly-bCategory",
    templateUrl: "./monthly-bCategory.html",
    animations: [stateAnimation]
})
export class MonthlyBCategoryBulletinComponent implements OnInit {
    @ViewChild("table") public grid: GridComponent;
    aggregates: AggregateDescriptor[] = [
        { field: "january", aggregate: "sum" },
        { field: "february", aggregate: "sum" },
        { field: "march", aggregate: "sum" },
        { field: "april", aggregate: "sum" },
        { field: "may", aggregate: "sum" },
        { field: "june", aggregate: "sum" },
        { field: "july", aggregate: "sum" },
        { field: "august", aggregate: "sum" },
        { field: "september", aggregate: "sum" },
        { field: "october", aggregate: "sum" },
        { field: "november", aggregate: "sum" },
        { field: "december", aggregate: "sum" },
        { field: "total", aggregate: "sum" }
    ];

    byType: GroupDescriptor[] = [{ field: 'type', aggregates: this.aggregates}];
    total: AggregateResult;
    groupable: GroupableSettings = { enabled: false, showFooter: true };
    initiallyExpanded = false;
    expandedKeys: GroupKey[] = [];

    animate: boolean = true;
    gridData: DataResult;
    columns: any[];
    codeList: any[];

    title: string;
    resultFormTitle: string;
    calcLabel: string;
    autofitColumnsLabel: string;
    yesLabel: string;
    noLabel: string;

    resultForm = new FormGroup({});
    resultOptions: FormlyFormOptions = {};
    resultFields: FormlyFieldConfig[];
    resultModel: any;

    remodelingCostsData: any[];
    remodelingCostsColumns: any[];
    remodelingCostsTableTitle: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    salesLabel: string;
    purchasesLabel: string;
    expensesLabel: string;
    noticeLabel: string;

    totalLabel: string

    constructor(
        private ngZone: NgZone,
        private uow: MonthlyBCategoryBulletinUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {

        this.yesLabel = this.translationService.translate('common.yes');
        this.noLabel = this.translationService.translate('common.no');
        this.calcLabel = this.translationService.translate('common.calc');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
        this.totalLabel = this.translationService.translate('common.aggregates');

        this.salesLabel = this.translationService.translate('query.1.Sales');
        this.purchasesLabel = this.translationService.translate('query.2.Purchases');
        this.expensesLabel = this.translationService.translate('query.3.Expenses');
        this.noticeLabel = this.translationService.translate('form.bCategoryMessage');
    }

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    public onDataStateChange(): void {
        this.fitColumns();
    }

    private fitColumns(): void {
        this.ngZone.onStable
            .asObservable()
            .pipe(take(1))
            .subscribe(() => {
                this.grid.autoFitColumns();
            });
    }

    ngOnInit(): void {
        this.uow.loadProperties()
            .then((result: any) => {
                result.searchModel['period'] = new Date(result.searchModel['period']);

                this.title = result.tableModel.customProperties.title;
                this.columns = result.tableModel.customProperties.columns;

                this.searchFields = result.searchModel.customProperties.fields;
                this.searchModel = result.searchModel;

                this.resultFields = [
                    {
                        fieldGroupClassName: 'grid',
                        fieldGroup: [
                            {
                                fieldGroupClassName: 'grid',
                                className: 'col-12 md:col-6',
                                fieldGroup: result.resultForm.customProperties.fields
                            }
                        ],
                    }
                ];
                this.resultFormTitle = result.resultForm.customProperties.title;
                this.remodelingCostsColumns = result.remodelingCostsTable.customProperties.columns;
                this.remodelingCostsTableTitle = result.remodelingCostsTable.customProperties.title;

                const fieldProperties = getFieldProperties(this.searchFields);

                if (fieldProperties['traderId']) {
                    fieldProperties['traderId'].props['selectionChange'] = (field: FormlyFieldConfig, trader: any) => {

                        if (trader) {
                            this.uow.traderChanged(trader.id, this.searchModel.period)
                                .then((result: any) => {
                                    field.form.get('expirationInventory').setValue(result.expirationInventory);
                                    field.form.get('expirationDepreciate').setValue(result.expirationDepreciate);

                                }).catch((err) => {
                                    throw err;
                                });
                        } else {
                            field.form.get('expirationInventory').setValue(0);
                            field.form.get('expirationDepreciate').setValue(0);
                        }
                    };
                }

                fieldProperties['period'].props['valueChange'] = (field: FormlyFieldConfig, date: Date) => {

                    if (this.searchModel.traderId > 0) {
                        this.uow.traderChanged(this.searchModel.traderId, date)
                            .then((result: any) => {
                                field.form.get('expirationInventory').setValue(result.expirationInventory);
                                field.form.get('expirationDepreciate').setValue(result.expirationDepreciate);

                            }).catch((err) => {
                                throw err;
                            });
                    } else {
                        field.form.get('expirationInventory').setValue(0);
                        field.form.get('expirationDepreciate').setValue(0);
                    }
                };

            }).catch((error: Error) => {
                throw error;
            });
    }

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((data: any) => {// Define a custom order
                this.gridData = process(data.codeList, { group: this.byType });

                this.resultModel = data.resultModel;
                this.remodelingCostsData = data.remodelingCostsList;

                this.onDataStateChange();

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    exportToPdf() {
        const model = {
            searchModel: this.searchModel,
            resultModel: this.resultModel
        };

        this.uow.exportToPdf(model)
            .then((result) => {
                if (result)
                    saveAs(result, "monthlyBCategoryBulletin.pdf");
                else
                    this.toastrService.error('Pdf file cannot be created.');
            }).catch((err: Error) => {
                throw err;
            });
    }
    get canExport() {
        return this.gridData && this.gridData.data.length > 0;
    }

    get canCalc() {
        const exist = typeof this.searchModel?.traderId === "number";
        return exist ? this.searchModel.traderId > 0 : false;
    }
}
