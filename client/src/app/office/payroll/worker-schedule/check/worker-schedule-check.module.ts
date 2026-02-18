import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkerScheduleCheckRoutingModule } from './worker-schedule-check-routing.module';
import { WorkerScheduleCheckComponent } from './components/worker-schedule-check';
import { FormListModule, FormlyEditModule, WorkerScheduleCheckUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        WorkerScheduleCheckRoutingModule
    ],
    declarations: [
        WorkerScheduleCheckComponent
    ],
    providers: [
        WorkerScheduleCheckUnitOfWork
    ]
})
export class WorkerScheduleCheckModule { }
