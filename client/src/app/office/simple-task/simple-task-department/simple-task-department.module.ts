import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SimpleTaskDepartmentRoutingModule } from './simple-task-department-routing.module';
import { SimpleTaskDepartmentListComponent } from './components/simple-task-department-list';
import { SimpleTaskDepartmentEditComponent } from './components/simple-task-department-edit';
import { FormListModule, FormlyEditModule, SimpleTaskDepartmentUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        SimpleTaskDepartmentRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        SimpleTaskDepartmentListComponent,
        SimpleTaskDepartmentEditComponent
    ],
    providers: [
        SimpleTaskDepartmentUnitOfWork
    ]
})
export class SimpleTaskDepartmentModule { }
