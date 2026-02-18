import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ScriptTraderRoutingModule } from './trader-routing.module';
import { ScriptTraderListComponent } from './components/trader-list';
import { ScriptTraderEditComponent } from './components/trader-edit';
import {
    FormListModule, FormlyEditModule,
    ScriptTraderUnitOfWork, ScriptTableUnitOfWork, ScriptTableItemUnitOfWork, ScriptFieldUnitOfWork,
    ScriptUnitOfWork, ScriptItemUnitOfWork, ScriptPivotUnitOfWork, ScriptPivotItemUnitOfWork, ScriptGroupUnitOfWork,
    FormlyEditDialogModule, ScriptToolUnitOfWork, ScriptToolItemUnitOfWork,
    OfficeSharedModule, FormListDetailDialogModule, UploadToolModule
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        ScriptTraderRoutingModule,
        FormListModule,
        FormlyEditModule,
        FormListDetailDialogModule,
        FormlyEditDialogModule,
        UploadToolModule
    ],
    declarations: [
        ScriptTraderListComponent,
        ScriptTraderEditComponent
    ],
    providers: [
        ScriptTraderUnitOfWork,
        ScriptTableUnitOfWork,
        ScriptTableItemUnitOfWork,
        ScriptFieldUnitOfWork,
        ScriptUnitOfWork,
        ScriptItemUnitOfWork,
        ScriptPivotUnitOfWork,
        ScriptPivotItemUnitOfWork,
        ScriptGroupUnitOfWork,
        ScriptToolUnitOfWork,
        ScriptToolItemUnitOfWork
    ]
})
export class ScriptTraderModule { }
