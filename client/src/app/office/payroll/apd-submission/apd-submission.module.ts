import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ApdSubmissionRoutingModule } from './apd-submission-routing.module';
import { ApdSubmissionComponent } from './components/apd-submission';
import { FormlyEditModule, ApdSubmissionUnitOfWork, OfficeSharedModule, StickyModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormlyEditModule,
        ApdSubmissionRoutingModule,
        StickyModule
    ],
    declarations: [
        ApdSubmissionComponent
    ],
    providers: [
        ApdSubmissionUnitOfWork
    ]
})
export class ApdSubmissionModule { }
