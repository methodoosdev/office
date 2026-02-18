import { Component, OnInit } from "@angular/core";

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { TraderRatingReportUnitOfWork } from "@officeNg";
import { AggregateDescriptor, AggregateResult, CompositeFilterDescriptor, DataResult, GroupDescriptor, SortDescriptor, distinct, process, orderBy } from "@progress/kendo-data-query";
import { IntlService } from "@progress/kendo-angular-intl";

@Component({
    selector: "by-valuation-table",
    templateUrl: "./list.html",
    animations: [stateAnimation]
})
export class TraderRatingByValuationTableComponent implements OnInit {
    animate: boolean = true;
    columns: any[];
    
    title: string;
    autofitColumnsLabel: string;

    public filter: CompositeFilterDescriptor = { logic: "and", filters: [] };
    public origin: any[];
    public gridData: DataResult;
    public employees: any[];

    aggregates: AggregateDescriptor[] = [
        { field: "turnover", aggregate: "sum" },
        //{ field: "turnoverRate", aggregate: "sum" },
        { field: "traderPayment", aggregate: "sum" },
        //{ field: "rateDep2", aggregate: "sum" },
        { field: "salaryDep2", aggregate: "sum" },
        //{ field: "rateDep3", aggregate: "sum" },
        { field: "salaryDep3", aggregate: "sum" },
        //{ field: "rateDep7", aggregate: "sum" },
        { field: "salaryDep7", aggregate: "sum" },
        //{ field: "rateDep8", aggregate: "sum" },
        { field: "salaryDep8", aggregate: "sum" },
        { field: "total", aggregate: "sum" },
        //{ field: "totalRate", aggregate: "average" }
    ];
    byType: GroupDescriptor[] = [{ field: 'specialtyName', aggregates: this.aggregates }];
    total: AggregateResult;
    turnoverTotal: AggregateResult;
    salaryDep2Total: AggregateResult;
    salaryDep3Total: AggregateResult;
    salaryDep7Total: AggregateResult;
    salaryDep8Total: AggregateResult;
    constructor(public intl: IntlService,
        private uow: TraderRatingReportUnitOfWork,
        private translationService: TranslationService) {

        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    loadData(data: any[]) {
        this.gridData = process(data, { group: this.byType, filter: this.filter });
        this.total = this.gridData.data.reduce((accumulator, obj) => {
            return accumulator + obj.aggregates.total.sum;
        }, 0);
        this.turnoverTotal = this.gridData.data.reduce((accumulator, obj) => {
            return accumulator + obj.aggregates.turnover.sum;
        }, 0);
        this.salaryDep2Total = this.gridData.data.reduce((accumulator, obj) => {
            return accumulator + obj.aggregates.salaryDep2.sum;
        }, 0);
        this.salaryDep3Total = this.gridData.data.reduce((accumulator, obj) => {
            return accumulator + obj.aggregates.salaryDep3.sum;
        }, 0);
        this.salaryDep7Total = this.gridData.data.reduce((accumulator, obj) => {
            return accumulator + obj.aggregates.salaryDep7.sum;
        }, 0);
        this.salaryDep8Total = this.gridData.data.reduce((accumulator, obj) => {
            return accumulator + obj.aggregates.salaryDep8.sum;
        }, 0);
    }

    ngOnInit(): void {

        this.uow.byValuationTable()
            .then((result: any) => {

                this.title = result.title;
                this.columns = result.columns;
                this.origin = [...result.data];
                const employees = distinct(this.origin, "employee").map((x, i) => {
                    return {
                        employeeId: x.employee,
                        employeeName: x.employee
                    }
                });
                const sort: SortDescriptor[] = [
                    { field: 'employeeName', dir: 'asc' } // Sort by 'employeeId' in ascending order
                ];

                this.employees = orderBy(employees, sort);

                this.loadData([...result.data]);

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.gridData?.data?.length > 0;
    }

    public filterChange(filter: CompositeFilterDescriptor): void {
        this.filter = filter;
        this.loadData(this.origin);
    }
}
