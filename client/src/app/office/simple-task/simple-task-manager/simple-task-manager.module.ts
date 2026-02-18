import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SimpleTaskManagerRoutingModule } from './simple-task-manager-routing.module';
import { SimpleTaskManagerListComponent } from './components/simple-task-manager-list';
import { SimpleTaskManagerEditComponent } from './components/simple-task-manager-edit';
import { FormListModule, FormlyEditModule, SimpleTaskManagerUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        SimpleTaskManagerRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        SimpleTaskManagerListComponent,
        SimpleTaskManagerEditComponent
    ],
    providers: [
        SimpleTaskManagerUnitOfWork
    ]
})
export class SimpleTaskManagerModule { }
