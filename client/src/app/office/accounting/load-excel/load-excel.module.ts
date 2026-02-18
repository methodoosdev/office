import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { FormListModule, FormlyEditModule, OfficeSharedModule } from '@officeNg';
import { MedicalExamUnitOfWork } from '@officeNg';

import { LoadExcelRoutingModule } from './load-excel-routing.module';
import { LoadExcelComponent } from './components/load-excel';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        LoadExcelRoutingModule
    ],
    declarations: [
        LoadExcelComponent
    ],
    providers: [
        MedicalExamUnitOfWork
    ]
})
export class LoadExcelModule { }
