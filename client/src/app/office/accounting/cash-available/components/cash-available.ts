import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { FormlyFieldConfig, FormlyFormOptions } from "@ngx-formly/core";
import { ToastrService } from "ngx-toastr";
import { AggregateDescriptor, AggregateResult, DataResult, GroupDescriptor, process } from "@progress/kendo-data-query";

import { stateAnimation } from "@primeNg";
import { getFieldProperties } from "@formlyNg";
import { CashAvailableUnitOfWork } from '@officeNg';
import { TranslationService } from "@core";
import { distinctUntilChanged, map, startWith } from "rxjs";

@Component({
    selector: "cash-available",
    templateUrl: "./cash-available.html",
    animations: [stateAnimation]
})
export class CashAvailableComponent implements OnInit {
    aggregates: AggregateDescriptor[] = [
        { field: "total", aggregate: "sum" }
    ];
    byType: GroupDescriptor[] = [{ field: 'type', aggregates: this.aggregates }];
    total: AggregateResult;

    animate: boolean = true;
    gridData: DataResult;
    columns: any[];

    title: string;
    calcLabel: string;
    traderLabel: string;
    autofitColumnsLabel: string;

    searchForm = new FormGroup({});
    searchOptions: FormlyFormOptions = {};
    searchFields: FormlyFieldConfig[];
    searchModel: any;

    traderName: any;
    grandTotalLabel: string;
    cashLabel: string;
    bankLabel: string;
    termLabel: string;

    constructor(
        private uow: CashAvailableUnitOfWork,
        private toastrService: ToastrService,
        private translationService: TranslationService) {
    }

    modelChangeEvent(value: any) {
        this.gridData = undefined;
    }

    ngOnInit(): void {

        this.title = this.translationService.translate('menu.cashAvailable');
        this.calcLabel = this.translationService.translate('common.calc');
        this.traderLabel = this.translationService.translate('common.trader');
        this.autofitColumnsLabel = this.translationService.translate('common.autofit');

        this.grandTotalLabel = this.translationService.translate('common.grandTotal');
        this.cashLabel = this.translationService.translate('query.1.Cash');
        this.bankLabel = this.translationService.translate('query.2.Bank');
        this.termLabel = this.translationService.translate('query.3.Term');

        //this.traderName = this.api.configuration.trader?.fullName;

        this.uow.loadProperties()
            .then((data: any) => {
                //this.title = data.tableModel.customProperties.title;
                this.columns = data.tableModel.customProperties.columns;

                this.searchFields = data.searchModel.customProperties.fields;
                this.searchModel = data.searchModel;

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

            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });

    }

    //ngAfterViewInit(): void {
    //    setTimeout(() => {
    //        const fields = getFieldProperties(this.searchFields);

    //        fields['period'].props['minDate'] = new Date(fields['period'].props['minDate']);
    //        fields['period'].props['maxDate'] = new Date(fields['period'].props['maxDate']);

    //    },100);
    //}

    calc() {
        this.uow.loadDataSource(this.searchModel)
            .then((data: any) => {
                this.gridData = process(data, { group: this.byType });
                this.total = this.gridData.data.reduce((accumulator, obj) => {
                    return accumulator + obj.aggregates.total.sum;
                }, 0);
            }).catch((e: any) => {
                this.toastrService.error(e.error);
            });
    }

    get canExport() {
        return this.gridData && this.gridData.data.length > 0;
    }

    get canCalc() {
        return this.searchModel && this.searchModel.traderId > 0;
    }
}
