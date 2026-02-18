import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkingAreaRoutingModule } from './working-area-routing.module';
import { WorkingAreaListComponent } from './components/working-area-list';
import { WorkingAreaEditComponent } from './components/working-area-edit';
import { FormListModule, FormlyEditModule, WorkingAreaUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        WorkingAreaRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        WorkingAreaListComponent,
        WorkingAreaEditComponent
    ],
    providers: [
        WorkingAreaUnitOfWork
    ]
})
export class WorkingAreaModule { }
