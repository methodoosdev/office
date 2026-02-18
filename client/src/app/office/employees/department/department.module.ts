import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { DepartmentRoutingModule } from './department-routing.module';
import { DepartmentListComponent } from './components/department-list';
import { DepartmentEditComponent } from './components/department-edit';
import { FormListModule, FormlyEditNewModule, DepartmentUnitOfWork, OfficeSharedModule } from '@officeNg';
//bugathina
@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        DepartmentRoutingModule,
        FormListModule,
        FormlyEditNewModule
    ],
    declarations: [
        DepartmentListComponent,
        DepartmentEditComponent
    ],
    providers: [
        DepartmentUnitOfWork
    ]
})
export class DepartmentModule { }
