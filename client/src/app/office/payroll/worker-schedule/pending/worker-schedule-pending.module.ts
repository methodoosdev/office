import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkerSchedulePendingRoutingModule } from './worker-schedule-pending-routing.module';
import { WorkerSchedulePendingListComponent } from './components/worker-schedule-pending-list';
import { PrimeSharedModule } from '@primeNg';
import { FormListModule, FormlyEditModule, WorkerSchedulePendingUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        PrimeSharedModule,
        WorkerSchedulePendingRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        WorkerSchedulePendingListComponent
    ],
    providers: [
        WorkerSchedulePendingUnitOfWork
    ]
})
export class WorkerSchedulePendingModule { }
