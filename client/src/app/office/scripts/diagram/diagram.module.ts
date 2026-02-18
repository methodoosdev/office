import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ScriptDiagramRoutingModule } from './diagram-routing.module';
import { ScriptDiagramComponent } from "./components/diagram";
import {
    FormListModule, FormlyEditModule,
    ScriptTraderUnitOfWork,
    OfficeSharedModule
} from '@officeNg';
import { DiagramModule } from '@progress/kendo-angular-diagrams';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        ScriptDiagramRoutingModule,
        DiagramModule
    ],
    declarations: [
        ScriptDiagramComponent
    ],
    providers: [
        ScriptTraderUnitOfWork
    ]
})
export class ScriptDiagramModule { }
