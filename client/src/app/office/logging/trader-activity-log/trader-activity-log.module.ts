import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

import { TraderActivityLogRoutingModule } from "./trader-activity-log-routing.module";
import { TraderActivityLogComponent } from "./components/trader-activity-log";
import { FormListModule, FormlyEditModule, TraderActivityLogUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        //StickyModule,
        TraderActivityLogRoutingModule
    ],
    declarations: [
        TraderActivityLogComponent
    ],
    providers: [
        TraderActivityLogUnitOfWork
    ],
})
export class TraderActivityLogModule { }
