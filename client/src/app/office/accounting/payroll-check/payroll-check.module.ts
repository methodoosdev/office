import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { PayrollCheckRoutingModule } from "./payroll-check-routing.module";
import { PayrollCheckComponent } from "./components/payroll-check";
import { FormListModule, FormlyEditModule, PayrollCheckUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        StickyModule,
        PayrollCheckRoutingModule
    ],
    declarations: [
        PayrollCheckComponent
    ],
    providers: [
        PayrollCheckUnitOfWork
    ],
})
export class PayrollCheckModule { }
