import { Component, OnInit } from "@angular/core";

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { TraderRatingReportUnitOfWork } from "@officeNg";
import { AggregateDescriptor, AggregateResult, CompositeFilterDescriptor, DataResult, GroupDescriptor, SortDescriptor, distinct, process, orderBy } from "@progress/kendo-data-query";
import { IntlService } from "@progress/kendo-angular-intl";
import { SingleSortSettings } from "@progress/kendo-angular-grid";

@Component({
    selector: "by-valuation-trader",
    templateUrl: "./list.html",
    animations: [stateAnimation]
})
export class TraderRatingByValuationTraderComponent implements OnInit {
    animate: boolean = true;
    columns: any[];
    
    title: string;
    autofitColumnsLabel: string;

    public filter: CompositeFilterDescriptor = { logic: "and", filters: [] };
    public origin: any[];
    public gridData: any[];
    public traders: any[];

    sortable: SingleSortSettings = { allowUnsort: false, mode: 'single' };
    sort: SortDescriptor[] = [{
        field: 'trader',
        dir: 'asc'
    }];
    totals: number;
    incomes: number;
    expences: number;

    constructor(public intl: IntlService,
        private uow: TraderRatingReportUnitOfWork,
        private translationService: TranslationService) {

        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    loadData(data: any[]) {
        this.gridData = process(data, { filter: this.filter }).data;
        this.totals = this.gridData.reduce((sum, obj) => {
            return sum + obj.total;
        }, 0);
        this.incomes = this.gridData.reduce((sum, obj) => {
            return sum + obj.income;
        }, 0);
        this.expences = this.gridData.reduce((sum, obj) => {
            return sum + obj.expences;
        }, 0);
    }

    ngOnInit(): void {

        this.uow.byValuationTrader()
            .then((result: any) => {

                this.title = result.title;
                this.columns = result.columns;
                this.origin = [...result.data];
                const traders = distinct(this.origin, "trader").map((x, i) => {
                    return {
                        traderId: x.trader,
                        traderName: x.trader
                    }
                });

                const sort: SortDescriptor[] = [
                    { field: 'traderName', dir: 'asc' } // Sort by 'employeeId' in ascending order
                ];
                this.traders = orderBy(traders, sort);

                this.loadData([...result.data]);

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.gridData?.length > 0;
    }

    public filterChange(filter: CompositeFilterDescriptor): void {
        this.filter = filter;
        this.loadData(this.origin);
    }

    //public sortChange(sort: SortDescriptor[]): void {
    //    this.sort = sort;
    //    this.gridView = {
    //        data: orderBy(this.products, this.sort),
    //        total: this.products.length,
    //}

}
