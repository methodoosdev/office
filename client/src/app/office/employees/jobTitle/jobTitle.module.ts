import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { JobTitleRoutingModule } from './jobTitle-routing.module';
import { JobTitleListComponent } from './components/jobTitle-list';
import { JobTitleEditComponent } from './components/jobTitle-edit';
import { FormlyEditModule, FormListModule, JobTitleUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        JobTitleRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        JobTitleListComponent,
        JobTitleEditComponent
    ],
    providers: [
        JobTitleUnitOfWork
    ]
})
export class JobTitleModule { }
