import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { EducationRoutingModule } from './education-routing.module';
import { EducationListComponent } from './components/education-list';
import { EducationEditComponent } from './components/education-edit';
import { FormlyEditModule, FormListModule, EducationUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        EducationRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        EducationListComponent,
        EducationEditComponent
    ],
    providers: [
        EducationUnitOfWork
    ]
})
export class EducationModule { }
