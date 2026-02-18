import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ScriptTableNameRoutingModule } from './table-name-routing.module';
import { ScriptTableNameListComponent } from './components/table-name-list';
import { ScriptTableNameEditComponent } from './components/table-name-edit';
import { FormListModule, FormlyEditModule, ScriptTableNameUnitOfWork, OfficeSharedModule } from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        ScriptTableNameRoutingModule,
        FormListModule,
        FormlyEditModule
    ],
    declarations: [
        ScriptTableNameListComponent,
        ScriptTableNameEditComponent
    ],
    providers: [
        ScriptTableNameUnitOfWork
    ]
})
export class ScriptTableNameModule { }
