import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";
import { IntlService } from "@progress/kendo-angular-intl";

import { stateAnimation } from "@primeNg";
import { getFieldProperties } from "@formlyNg";
import { IntertemporalCUnitOfWork } from '@officeNg';
import { TranslationService } from "@core";
import { ConfiguratorSettings, Dimension, Measure, PivotGridAxis, sumAggregate } from "@progress/kendo-angular-pivotgrid";

export interface DataItem {
    description: string;
    year: string;
    value: number;
}

@Component({
    selector: "intertemporal-c",
    templateUrl: "./intertemporal-c.html",
    //styleUrls: ["./intertemporal-c.css"],
    styles: [
        `
            .k-pivotgrid-table .k-pivotgrid-header-root {
                display: none;
            }
            .k-first.k-pivotgrid-cell {
                width: 100%;
            }
            .k-pivotgrid-column-headers {
                text-align: center;
            }
            .k-pivotgrid-values {
                text-align: right;
            }
        `],
    animations: [stateAnimation],
    encapsulation: ViewEncapsulation.None,
})

export class IntertemporalCComponent implements OnInit {
    animate: boolean = true;
    gridData: any[];
    columns: any[];

    title: string;
    resultFormTitle: string;
    calcLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    resultForm = new FormGroup({});
    resultOptions: FormlyFormOptions = {};
    resultFields: FormlyFieldConfig[];
    resultModel: any;

    remodelingCostsData: any[];
    remodelingCostsColumns: any[];
    remodelingCostsTableTitle: string;

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    constructor(
        private intl: IntlService,
        private uow: IntertemporalCUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
            
        this.calcLabel = this.translationService.translate('common.calc');
        this.traderLabel = this.translationService.translate('common.trader');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    formatNumber(value: number) {
        return value ? this.intl.formatNumber(value, "n2") : undefined;
    }

    ngOnInit(): void {

        this.uow.loadProperties()
            .then((result: any) => {
                this.title = result.searchModel.customProperties.title;

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
                            this.uow.traderChanged(trader.id)
                                .then((result: any) => {
                                    field.form.get('expirationInventory').setValue(result.expirationInventory);
                                    field.form.get('expirationDepreciate').setValue(result.expirationDepreciate);

                                }).catch((err) => {
                                    this.gridData = undefined;
                                    throw err;
                                });
                        } else {
                            field.form.get('expirationInventory').setValue(0);
                            field.form.get('expirationDepreciate').setValue(0);
                        }
                    };
                }

            }).catch((e: any) => {
                this.gridData = undefined;
                this.toastrService.error(e.error);
            });
    }

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((result: any) => {
                this.gridData = result.pivot;

                this.resultModel = result.resultModel;
                this.remodelingCostsData = result.remodelingCostsList;

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

    settings: ConfiguratorSettings = {
        position: "left",
        orientation: "vertical",
    };

    dimensions: { [key: string]: Dimension } = {
        year: {
            caption: "year", //this.translationService.translate('common.year'),
            displayValue: (item) => item.year,
            sortValue: (displayValue: string) => displayValue,
        },
        description: {
            caption: "description",
            displayValue: (item) => item.description,
            sortValue: (displayValue: string) => displayValue,
        },
    };
    
    measures: Measure[] = [
        {
            name: 'total',
            value: (item: DataItem): number => item.value,
            aggregate: sumAggregate,
        }
    ];

    defaultMeasureAxes: PivotGridAxis[] = [{ name: ['total'] }];

    defaultRowAxes: PivotGridAxis[] = [{ name: ['description'], expand: true }];

    defaultColumnAxes: PivotGridAxis[] = [{ name: ['year'], expand: true }];
}

  
