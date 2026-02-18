import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { EmployeeRoutingModule } from './employee-routing.module';
import { EmployeeListComponent } from './components/employee-list';
import { EmployeeEditComponent } from './components/employee-edit';
import {
    EmployeeUnitOfWork, TradersByEmployeeUnitOfWork, TraderUnitOfWork, FormlyEditModule, FormListModule,
    FormListDetailMappingModule, FormListDialogModule, OfficeSharedModule
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        EmployeeRoutingModule,
        FormListModule,
        FormlyEditModule,
        FormListDetailMappingModule,
        FormListDialogModule
    ],
    declarations: [
        EmployeeListComponent,
        EmployeeEditComponent
    ],
    providers: [
        EmployeeUnitOfWork,
        TradersByEmployeeUnitOfWork,
        TraderUnitOfWork
    ]
})
export class EmployeeModule { }
