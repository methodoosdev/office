import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { WorkerLeaveRoutingModule } from "./worker-leave-routing.module";
import { WorkerLeaveComponent } from "./components/worker-leave";
import { FormlyEditModule, WorkerLeaveUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        WorkerLeaveRoutingModule,
        FormlyEditModule,
        StickyModule
    ],
    declarations: [
        WorkerLeaveComponent
    ],
    providers: [
        WorkerLeaveUnitOfWork
    ],
})
export class WorkerLeaveModule { }
