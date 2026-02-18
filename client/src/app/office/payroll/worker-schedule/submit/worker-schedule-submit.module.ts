import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkerScheduleSubmitRoutingModule } from './worker-schedule-submit-routing.module';
import { WorkerScheduleSubmitComponent } from './components/worker-schedule-submit';
import { FormListModule, FormlyEditModule, WorkerScheduleSubmitUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        WorkerScheduleSubmitRoutingModule
    ],
    declarations: [
        WorkerScheduleSubmitComponent
    ],
    providers: [
        WorkerScheduleSubmitUnitOfWork
    ]
})
export class WorkerScheduleSubmitModule { }
