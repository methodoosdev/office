import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ScriptPivotRoutingModule } from './pivot-routing.module';
import { ScriptPivotListComponent } from "./components/pivot-list";
import {
    FormListModule, FormlyEditModule,
    ScriptTraderUnitOfWork,
    OfficeSharedModule
} from '@officeNg';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        ScriptPivotRoutingModule
    ],
    declarations: [
        ScriptPivotListComponent
    ],
    providers: [
        ScriptTraderUnitOfWork
    ]
})
export class ScriptPivotModule { }
