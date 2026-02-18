import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { WorkerSickLeaveRoutingModule } from "./worker-sick-leave-routing.module";
import { WorkerSickLeaveComponent } from "./components/worker-sick-leave";
import { FormlyEditModule, WorkerSickLeaveUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormlyEditModule,
        WorkerSickLeaveRoutingModule,
        StickyModule
    ],
    declarations: [
        WorkerSickLeaveComponent
    ],
    providers: [
        WorkerSickLeaveUnitOfWork
    ],
})
export class WorkerSickLeaveModule { }
