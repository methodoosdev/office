import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SpecialtyRoutingModule } from './specialty-routing.module';
import { SpecialtyListComponent } from './components/specialty-list';
import { SpecialtyEditComponent } from './components/specialty-edit';
import { FormlyEditModule, FormListModule, SpecialtyUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        SpecialtyRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        SpecialtyListComponent,
        SpecialtyEditComponent
    ],
    providers: [
        SpecialtyUnitOfWork
    ]
})
export class SpecialtyModule { }
