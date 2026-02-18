import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkerLeaveDetailRoutingModule } from './leave-detail-routing.module';
import { WorkerLeaveDetailListComponent } from './components/leave-detail-list';
import { WorkerLeaveDetailEditComponent } from './components/leave-detail-edit';
import { FormlyEditModule, FormListModule, WorkerLeaveDetailUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        WorkerLeaveDetailRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        WorkerLeaveDetailListComponent,
        WorkerLeaveDetailEditComponent
    ],
    providers: [
        WorkerLeaveDetailUnitOfWork
    ]
})
export class WorkerLeaveDetailModule { }
