import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TraderRatingByDepartmentComponent } from './by-department/list';
import { TraderRatingByEmployeeComponent } from './by-employee/list';
import { TraderRatingByTraderComponent } from './by-trader/list';
import { TraderRatingBySummaryTableComponent } from './by-summaryTable/list';
import { TraderRatingByValuationTableComponent } from './by-valuationTable/list';
import { TraderRatingByValuationTraderComponent } from './by-validationTrader/list';

const routes: Routes = [
    {
        path: '',
        children: [
            { path: 'by-valuation-trader', component: TraderRatingByValuationTraderComponent },
            { path: 'by-valuation-table', component: TraderRatingByValuationTableComponent },
            { path: 'by-summary-table', component: TraderRatingBySummaryTableComponent },
            { path: 'by-department', component: TraderRatingByDepartmentComponent },
            { path: 'by-employee', component: TraderRatingByEmployeeComponent },
            { path: 'by-trader', component: TraderRatingByTraderComponent }
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class TraderRatingReportRoutingModule { }
