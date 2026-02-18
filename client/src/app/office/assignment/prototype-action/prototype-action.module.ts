import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AssignmentPrototypeActionUnitOfWork, FormListModule, FormlyEditModule, OfficeSharedModule } from '@officeNg';

import { AssignmentPrototypeActionRoutingModule } from './prototype-action-routing.module';
import { AssignmentPrototypeActionListComponent } from './components/prototype-action-list';
import { AssignmentPrototypeActionEditComponent } from './components/prototype-action-edit';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        AssignmentPrototypeActionRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        AssignmentPrototypeActionListComponent,
        AssignmentPrototypeActionEditComponent
    ],
    providers: [
        AssignmentPrototypeActionUnitOfWork
    ]
})
export class AssignmentPrototypeActionModule { }
