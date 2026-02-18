import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkerScheduleLogRoutingModule } from './worker-schedule-log-routing.module';
import { WorkerScheduleLogListComponent } from './components/worker-schedule-log-list';
import { WorkerScheduleLogEditComponent } from './components/worker-schedule-log-edit';
import { FormListModule, FormlyEditModule, WorkerScheduleLogUnitOfWork, PersistStateUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        WorkerScheduleLogRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        WorkerScheduleLogListComponent,
        WorkerScheduleLogEditComponent
    ],
    providers: [
        WorkerScheduleLogUnitOfWork, PersistStateUnitOfWork
    ]
})
export class WorkerScheduleLogModule { }
