import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { TraderRatingReportRoutingModule } from './report-routing.module';
import { TraderRatingByDepartmentComponent } from './by-department/list';
import { TraderRatingByEmployeeComponent } from './by-employee/list';
import { TraderRatingByTraderComponent } from './by-trader/list';
import { TraderRatingReportUnitOfWork, OfficeSharedModule, MultiCheckFilterModule } from '@officeNg';
import { TraderRatingBySummaryTableComponent } from './by-summaryTable/list';
import { TraderRatingByValuationTableComponent } from './by-valuationTable/list';
import { TraderRatingByValuationTraderComponent } from './by-validationTrader/list';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        TraderRatingReportRoutingModule,
        MultiCheckFilterModule
    ],
    declarations: [
        TraderRatingByDepartmentComponent,
        TraderRatingByEmployeeComponent,
        TraderRatingByTraderComponent,
        TraderRatingBySummaryTableComponent,
        TraderRatingByValuationTableComponent,
        TraderRatingByValuationTraderComponent
    ],
    providers: [
        TraderRatingReportUnitOfWork
    ]
})
export class TraderRatingReportModule { }
