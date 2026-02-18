import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { PayrollStatusRoutingModule } from "./payroll-status-routing.module";
import { PayrollStatusComponent } from "./components/payroll-status";
import { FormlyEditModule, PayrollStatusUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        PayrollStatusRoutingModule,
        FormlyEditModule,
        StickyModule
    ],
    declarations: [
        PayrollStatusComponent
    ],
    providers: [
        PayrollStatusUnitOfWork
    ],
})
export class PayrollStatusModule { }
