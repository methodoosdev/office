import { Component, OnInit, ViewChild, NgZone } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ScriptTraderUnitOfWork } from "@officeNg";
import { GridColumn, GridResponse } from "../../api/public-api";
import { GridComponent, GridDataResult } from "@progress/kendo-angular-grid";
import { take } from "rxjs/operators";
import { AggregateResult, State, aggregateBy, process } from "@progress/kendo-data-query";
import { IntlService } from "@progress/kendo-angular-intl";

@Component({
    selector: "pivot-report-list",
    templateUrl: "./pivot-list.html"
})
export class ScriptPivotListComponent implements OnInit {
    @ViewChild(GridComponent) public grid: GridComponent;
    pathUrl = 'office/script-pivot';
    title: string;
    pivotData: GridDataResult;
    state: State = { filter: { logic: "and", filters: [] }, };
    pivotColumns: GridColumn[] = [];
    total: AggregateResult;
    showTypeId: number;

    constructor(
        private ngZone: NgZone,
        private intl: IntlService,
        private route: ActivatedRoute,
        public uow: ScriptTraderUnitOfWork) { }

    get canExport() {
        return this.pivotData?.data?.length > 0;
    }

    public getAggregate(col: GridColumn) {
        if (col.filter === 'numeric')
            return this.intl.formatNumber(this.total[col.field].sum, "#,##0.00");
        else
            return "Σύνολο";
    }

    ngOnInit(): void {
        const id = +this.route.snapshot.paramMap.get('id');
        const traderId = +this.route.snapshot.paramMap.get('parentId');
        const categoryBookTypeId = +this.route.snapshot.paramMap.get('categoryBookTypeId');
        const year = +this.route.snapshot.paramMap.get('year');
        const period = +this.route.snapshot.paramMap.get('period');
        const showTypeId = +this.route.snapshot.paramMap.get('showTypeId');
        const inventory = +this.route.snapshot.paramMap.get('inventory');

        this.uow.pivot({ id, traderId, categoryBookTypeId, year, period, showTypeId, inventory: (inventory === 1) ? true : false })
            .then((res: GridResponse<any>) => {
                this.title = res.title
                this.pivotData = process(res.data, this.state);
                this.pivotColumns = res.columns;

                this.showTypeId = showTypeId;

                if (showTypeId === 0)
                    this.total = aggregateBy(res.data, res.aggregates);

                setTimeout(() => {
                    this.fitColumns();
                }, 10);
            })
            .catch((err: Error) => {
                Promise.reject(err);
            });
    }

    private fitColumns(): void {
        this.ngZone.onStable
            .asObservable()
            .pipe(take(1))
            .subscribe(() => {
                this.grid.autoFitColumns();
            });
    }
}
