import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UploadsModule } from "@progress/kendo-angular-upload";

import { FormListModule, FormlyEditModule, OfficeSharedModule } from '@officeNg';
import { MedicalExamUnitOfWork } from '@officeNg';

import { MedicalExamRoutingModule } from './medical-exam-routing.module';
import { MedicalExamComponent } from './components/medical-exam';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        MedicalExamRoutingModule,
        UploadsModule
    ],
    declarations: [
        MedicalExamComponent
    ],
    providers: [
        MedicalExamUnitOfWork
    ]
})
export class MedicalExamModule { }
