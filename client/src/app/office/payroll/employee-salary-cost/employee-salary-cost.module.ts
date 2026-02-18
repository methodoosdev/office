import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { EmployeeSalaryCostRoutingModule } from "./employee-salary-cost-routing.module";
import { EmployeeSalaryCostComponent } from "./components/employee-salary-cost";
import { FormlyEditModule, EmployeeSalaryCostUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        EmployeeSalaryCostRoutingModule,
        FormlyEditModule,
        StickyModule
    ],
    declarations: [
        EmployeeSalaryCostComponent
    ],
    providers: [
        EmployeeSalaryCostUnitOfWork
    ],
})
export class EmployeeSalaryCostModule { }
