import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkerScheduleByEmployeeRoutingModule } from './routing-by-employee.module';
import { WorkerScheduleByEmployeeListComponent } from './components/list-by-employee';
import { WorkerScheduleByEmployeeEditComponent } from './components/edit-by-employee';
import { FormListModule, FormlyEditModule, WorkerScheduleByEmployeeUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        WorkerScheduleByEmployeeRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        WorkerScheduleByEmployeeListComponent,
        WorkerScheduleByEmployeeEditComponent
    ],
    providers: [
        WorkerScheduleByEmployeeUnitOfWork
    ]
})
export class WorkerScheduleByEmployeeModule { }
