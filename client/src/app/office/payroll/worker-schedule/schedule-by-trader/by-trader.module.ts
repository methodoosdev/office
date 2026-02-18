import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkerScheduleByTraderRoutingModule } from './routing-by-trader.module';
import { WorkerScheduleByTraderListComponent } from './components/list-by-trader';
import { WorkerScheduleByTraderEditComponent } from './components/edit-by-trader';
import { FormListModule, FormlyEditModule, WorkerScheduleByTraderUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        WorkerScheduleByTraderRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        WorkerScheduleByTraderListComponent,
        WorkerScheduleByTraderEditComponent
    ],
    providers: [
        WorkerScheduleByTraderUnitOfWork
    ]
})
export class WorkerScheduleByTraderModule { }
