import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { EmployeeActivityLogRoutingModule } from "./employee-activity-log-routing.module";
import { EmployeeActivityLogComponent } from "./components/employee-activity-log";
import { FormListModule, FormlyEditModule, EmployeeActivityLogUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        //StickyModule,
        EmployeeActivityLogRoutingModule
    ],
    declarations: [
        EmployeeActivityLogComponent
    ],
    providers: [
        EmployeeActivityLogUnitOfWork
    ],
})
export class EmployeeActivityLogModule { }
