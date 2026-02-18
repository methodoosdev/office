import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { VatExemptionApprovalRoutingModule } from './approval-routing.module';
import { VatExemptionApprovalListComponent } from './components/approval-list';
import { VatExemptionApprovalEditComponent } from './components/approval-edit';
import { FormListModule, FormlyEditModule, VatExemptionApprovalUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        VatExemptionApprovalRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        VatExemptionApprovalListComponent,
        VatExemptionApprovalEditComponent
    ],
    providers: [
        VatExemptionApprovalUnitOfWork
    ]
})
export class VatExemptionApprovalModule { }
