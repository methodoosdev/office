import { Component, OnInit } from "@angular/core";

import { TranslationService } from "@core";
import { stateAnimation } from "@primeNg";
import { TraderRatingReportUnitOfWork } from "@officeNg";
import { CompositeFilterDescriptor, SortDescriptor, distinct, filterBy, orderBy } from "@progress/kendo-data-query";

@Component({
    selector: "by-summary-table",
    templateUrl: "./list.html",
    animations: [stateAnimation]
})
export class TraderRatingBySummaryTableComponent implements OnInit {
    animate: boolean = true;
    columns: any[];
    
    title: string;
    autofitColumnsLabel: string;

    constructor(
        private uow: TraderRatingReportUnitOfWork,
        private translationService: TranslationService) {

        this.autofitColumnsLabel = this.translationService.translate('common.autofit');
    }

    ngOnInit(): void {
        this.uow.bySummaryTable()
            .then((result: any) => {

                this.title = result.title;
                this.columns = result.columns;
                this.origin = [...result.data];
                this.gridData = filterBy([...result.data], this.filter);
                const employees = distinct(this.origin, "employee_Dep2").map((x,i) => {
                    return {
                        employeeId: x.employee_Dep2,
                        employeeName: x.employee_Dep2
                    }
                });
                const sort: SortDescriptor[] = [
                    { field: 'employeeName', dir: 'asc' } // Sort by 'employeeId' in ascending order
                ];

                this.employees = orderBy(employees, sort);

            }).catch((error: Error) => {
                throw error;
            });
    }

    get canExport() {
        return this.gridData && this.gridData.length > 0;
    }

    public filter: CompositeFilterDescriptor = { logic: "and", filters: [] };
    public origin: any[];
    public gridData: any[];
    public employees: any[];

    public filterChange(filter: CompositeFilterDescriptor): void {
        this.filter = filter;
        this.gridData = filterBy(this.origin, filter);
    }
}
