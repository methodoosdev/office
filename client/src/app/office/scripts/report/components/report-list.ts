import { Component, NgZone, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ScriptTraderUnitOfWork } from "@officeNg";
import { GridColumn, GridResponse } from "../../api/public-api";
import { GridComponent, GroupableSettings } from "@progress/kendo-angular-grid";
import { take } from "rxjs";
import { GroupDescriptor, groupBy } from "@progress/kendo-data-query";

@Component({
    selector: "script-report-list",
    templateUrl: "./report-list.html"
})
export class ScriptReportListComponent implements OnInit {
    @ViewChild(GridComponent) public grid: GridComponent;
    pathUrl = 'office/script-report';
    title: string;
    pivotData: any[];
    pivotColumns: GridColumn[] = [];
    groupable: GroupableSettings = { enabled: false, showFooter: false };
    groups: GroupDescriptor[] = [{ field: "group", dir: 'asc' }];

    constructor(
        private ngZone: NgZone,
        private route: ActivatedRoute,
        public uow: ScriptTraderUnitOfWork) { }

    get canExport() {
        return this.pivotData && this.pivotData.length > 0;
    }

    ngOnInit(): void {
        const model: {
            traderId?: number;
            categoryBookTypeId?: number;
            config?: any;
            groups: string[];
        } = { groups: [] };
        //const traderId = +this.route.snapshot.paramMap.get('id');
        //const categoryBookTypeId = +this.route.snapshot.paramMap.get('categoryBookTypeId');

        // Path params
        this.route.paramMap.subscribe(pm => {
            model.traderId = +(pm.get('id') ?? 0);
            model.categoryBookTypeId = +(pm.get('categoryBookTypeId') ?? 0);
            model.config = pm.get('config');  
            //model.name = pm.get('name') ?? '';
        });

        // Query params (arrays come via getAll)
        this.route.queryParamMap.subscribe(qm => {
            model.groups = qm.getAll('groups');
        });

        // Query params (arrays come via getAll)
        //this.route.queryParamMap.subscribe(qm => {
        //    const raw = qm.getAll('ids');           // ["1","2","3"]
        //    model.ids = raw.map(x => Number(x));  // [1,2,3]
        //});

        this.uow.print(model.groups, model.traderId, model.categoryBookTypeId, model.config)
            .then((res: GridResponse<any>) => {
                this.title = res.title
                this.pivotColumns = res.columns;
                this.pivotData = groupBy(res.data, this.groups); res.data;

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
