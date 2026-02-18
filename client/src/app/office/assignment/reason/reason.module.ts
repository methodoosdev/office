import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AssignmentReasonUnitOfWork, FormListModule, FormlyEditModule, OfficeSharedModule } from '@officeNg';

import { AssignmentReasonRoutingModule } from './reason-routing.module';
import { AssignmentReasonListComponent } from './components/reason-list';
import { AssignmentReasonEditComponent } from './components/reason-edit';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        AssignmentReasonRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        AssignmentReasonListComponent,
        AssignmentReasonEditComponent
    ],
    providers: [
        AssignmentReasonUnitOfWork
    ]
})
export class AssignmentReasonModule { }
