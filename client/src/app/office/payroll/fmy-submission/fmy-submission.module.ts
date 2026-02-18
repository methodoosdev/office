import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { FmySubmissionRoutingModule } from './fmy-submission-routing.module';
import { FmySubmissionComponent } from './components/fmy-submission';
import { FormlyEditModule, FmySubmissionUnitOfWork, OfficeSharedModule, StickyModule } from "@officeNg";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,        
        FormlyEditModule,
        FmySubmissionRoutingModule,
        StickyModule
    ],
    declarations: [
        FmySubmissionComponent
    ],
    providers: [
        FmySubmissionUnitOfWork
    ]
})
export class FmySubmissionModule { }
