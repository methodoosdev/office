import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SimpleTaskCategoryRoutingModule } from './simple-task-category-routing.module';
import { SimpleTaskCategoryListComponent } from './components/simple-task-category-list';
import { SimpleTaskCategoryEditComponent } from './components/simple-task-category-edit';
import { FormListModule, FormlyEditModule, SimpleTaskCategoryUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        SimpleTaskCategoryRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        SimpleTaskCategoryListComponent,
        SimpleTaskCategoryEditComponent
    ],
    providers: [
        SimpleTaskCategoryUnitOfWork
    ]
})
export class SimpleTaskCategoryModule { }
