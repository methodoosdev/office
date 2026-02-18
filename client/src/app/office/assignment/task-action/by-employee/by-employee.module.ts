import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AssignmentTaskActionByEmployeeRoutingModule } from './by-employee-routing.module';
import { AssignmentTaskActionByEmployeeListComponent } from './components/by-employee-list';
import { AssignmentTaskActionByEmployeeEditComponent } from './components/by-employee-edit';
import { AssignmentTaskActionByEmployeeUnitOfWork, FormListModule, FormlyEditModule, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        AssignmentTaskActionByEmployeeRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        AssignmentTaskActionByEmployeeListComponent,
        AssignmentTaskActionByEmployeeEditComponent
    ],
    providers: [
        AssignmentTaskActionByEmployeeUnitOfWork
    ]
})
export class AssignmentTaskActionByEmployeeModule { }
