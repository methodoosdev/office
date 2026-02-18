import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SimpleTaskNatureRoutingModule } from './simple-task-nature-routing.module';
import { SimpleTaskNatureListComponent } from './components/simple-task-nature-list';
import { SimpleTaskNatureEditComponent } from './components/simple-task-nature-edit';
import { FormListModule, FormlyEditModule, SimpleTaskNatureUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        SimpleTaskNatureRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        SimpleTaskNatureListComponent,
        SimpleTaskNatureEditComponent
    ],
    providers: [
        SimpleTaskNatureUnitOfWork
    ]
})
export class SimpleTaskNatureModule { }
