import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AggregateAnalysisRoutingModule } from './aggregate-analysis-routing.module';
import { AggregateAnalysisComponent } from './components/aggregate-analysis';
import { FormListModule, FormlyEditModule, AggregateAnalysisUnitOfWork, OfficeSharedModule } from "@officeNg";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        AggregateAnalysisRoutingModule
    ],
    declarations: [
        AggregateAnalysisComponent
    ],
    providers: [
        AggregateAnalysisUnitOfWork
    ]
})
export class AggregateAnalysisModule { }
