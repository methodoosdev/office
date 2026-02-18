import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AssignmentPrototypeRoutingModule } from './prototype-routing.module';
import { AssignmentPrototypeListComponent } from './components/prototype-list';
import { AssignmentPrototypeEditComponent } from './components/prototype-edit';
import {
    AssignmentPrototypeActionsByAssignmentPrototypeUnitOfWork, AssignmentPrototypeActionUnitOfWork, AssignmentPrototypeUnitOfWork,
    OfficeSharedModule, FormListDetailMappingModule, FormListModule, FormlyEditModule
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        AssignmentPrototypeRoutingModule,
        FormListDetailMappingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        AssignmentPrototypeListComponent,
        AssignmentPrototypeEditComponent
    ],
    providers: [
        AssignmentPrototypeUnitOfWork,
        AssignmentPrototypeActionUnitOfWork,
        AssignmentPrototypeActionsByAssignmentPrototypeUnitOfWork
    ]
})
export class AssignmentPrototypeModule { }
