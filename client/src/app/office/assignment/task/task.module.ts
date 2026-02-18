import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AssignmentTaskRoutingModule } from './task-routing.module';
import { AssignmentTaskListComponent } from './components/task-list';
import { AssignmentTaskEditComponent } from './components/task-edit';
import { AssignmentTaskActionUnitOfWork, AssignmentTaskUnitOfWork, FormListDetailModule, FormListModule, FormlyEditModule, MoreLessModule, OfficeSharedModule } from '@officeNg';
import { TaskEditDialogComponent } from './components/dialog/task-edit-dialog';
import { TaskEditDetailComponent } from './components/detail/task-edit-detail';
import { AvatarModule, PrimeSharedModule } from '@primeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        PrimeSharedModule,
        AvatarModule,
        MoreLessModule,
        AssignmentTaskRoutingModule,
        FormListDetailModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        TaskEditDetailComponent,
        TaskEditDialogComponent,
        AssignmentTaskListComponent,
        AssignmentTaskEditComponent
    ],
    providers: [
        AssignmentTaskUnitOfWork,
        AssignmentTaskActionUnitOfWork
    ]
})
export class AssignmentTaskModule { }
