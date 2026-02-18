import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ScriptToolRoutingModule } from './tool-routing.module';
import { ScriptToolListComponent } from "./components/tool-list";
import {
    FormListModule, FormlyEditModule,
    ScriptTraderUnitOfWork,
    OfficeSharedModule
} from '@officeNg';
import { LandscapeToolComponent, FontSizeToolComponent } from './components/custom-tools';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        OfficeSharedModule,
        FormListModule,
        FormlyEditModule,
        ScriptToolRoutingModule
    ],
    declarations: [
        ScriptToolListComponent,
        LandscapeToolComponent, FontSizeToolComponent
    ],
    providers: [
        ScriptTraderUnitOfWork
    ]
})
export class ScriptToolModule { }
