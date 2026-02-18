import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { VatExemptionReportRoutingModule } from './report-routing.module';
import { VatExemptionReportListComponent } from './components/report-list';
import { VatExemptionReportEditComponent } from './components/report-edit';
import { FormListModule, FormlyEditModule, VatExemptionReportUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        VatExemptionReportRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        VatExemptionReportListComponent,
        VatExemptionReportEditComponent
    ],
    providers: [
        VatExemptionReportUnitOfWork
    ]
})
export class VatExemptionReportModule { }
