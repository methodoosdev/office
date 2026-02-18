import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ScriptReportRoutingModule } from './report-routing.module';
import { ScriptReportListComponent } from './components/report-list';
import {
    FormListModule, FormlyEditModule,
    ScriptTraderUnitOfWork,
    OfficeSharedModule
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        ScriptReportRoutingModule
    ],
    declarations: [
        ScriptReportListComponent
    ],
    providers: [
        ScriptTraderUnitOfWork
    ]
})
export class ScriptReportModule { }
