import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SimpleTaskSectorRoutingModule } from './simple-task-sector-routing.module';
import { SimpleTaskSectorListComponent } from './components/simple-task-sector-list';
import { SimpleTaskSectorEditComponent } from './components/simple-task-sector-edit';
import { FormListModule, FormlyEditModule, SimpleTaskSectorUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        SimpleTaskSectorRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        SimpleTaskSectorListComponent,
        SimpleTaskSectorEditComponent
    ],
    providers: [
        SimpleTaskSectorUnitOfWork
    ]
})
export class SimpleTaskSectorModule { }
