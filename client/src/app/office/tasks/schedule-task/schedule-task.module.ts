import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ScheduleTaskRoutingModule } from './schedule-task-routing.module';
import { ScheduleTaskListComponent } from './components/schedule-task-list';
import { ScheduleTaskEditComponent } from './components/schedule-task-edit';
import { FormlyEditModule, FormListModule, ScheduleTaskUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        ScheduleTaskRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        ScheduleTaskListComponent,
        ScheduleTaskEditComponent
    ],
    providers: [
        ScheduleTaskUnitOfWork
    ]
})
export class ScheduleTaskModule { }
