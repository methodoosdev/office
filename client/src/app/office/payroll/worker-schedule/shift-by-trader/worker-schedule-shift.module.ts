import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkerScheduleShiftRoutingModule } from './worker-schedule-shift-routing.module';
import { WorkerScheduleShiftListComponent } from './components/worker-schedule-shift-list';
import { WorkerScheduleShiftEditComponent } from './components/worker-schedule-shift-edit';
import { FormListModule, FormlyEditModule, WorkerScheduleShiftByTraderUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        WorkerScheduleShiftRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        WorkerScheduleShiftListComponent,
        WorkerScheduleShiftEditComponent
    ],
    providers: [
        WorkerScheduleShiftByTraderUnitOfWork
    ]
})
export class WorkerScheduleShiftModule { }
